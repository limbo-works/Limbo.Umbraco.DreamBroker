// [CHANGE: Umbraco 17 upgrade - UmbracoAuthorizedApiController and [PluginController] were removed with the AngularJS
// backoffice. This is now a versioned Management API controller under /umbraco/management/api/v1/limbo/dreambroker,
// and results are serialized with Newtonsoft (via NewtonsoftJsonResult) because our models use [JsonProperty]]
// Related: Api/DreamBrokerApiConstants.cs, Services/DreamBrokerService.cs, wwwroot/Service.js

using System;
using System.Collections.Generic;
using System.Linq;
using Asp.Versioning;
using Limbo.Umbraco.DreamBroker.Api;
using Limbo.Umbraco.DreamBroker.Models.Channels;
using Limbo.Umbraco.DreamBroker.Models.Videos;
using Limbo.Umbraco.DreamBroker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skybrud.Essentials.AspNetCore.Json.Newtonsoft;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.DreamBroker.Controllers;

[ApiController]
[VersionedApiBackOfficeRoute(DreamBrokerApiConstants.Route)]
// The data type can be used on document, media and member types alike, so gating on Content section access would
// lock out editors who only have Media or Members access
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
[MapToApi(DreamBrokerApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = DreamBrokerApiConstants.GroupName)]
public class DreamBrokerController : ManagementApiControllerBase {

    private readonly ILogger<DreamBrokerController> _logger;
    private readonly DreamBrokerService _dreamBrokerService;

    #region Constructors

    public DreamBrokerController(ILogger<DreamBrokerController> logger, DreamBrokerService dreamBrokerService) {
        _logger = logger;
        _dreamBrokerService = dreamBrokerService;
    }

    #endregion

    #region Public API methods

    /// <summary>
    /// Returns the server variables needed by the client side part of this package.
    /// </summary>
    [HttpGet("serverVariables")]
    public IActionResult GetServerVariables() {
        return Ok(new {
            version = DreamBrokerPackage.InformationalVersion,
            cacheBuster = DreamBrokerPackage.CacheBuster
        });
    }

    /// <summary>
    /// Returns a list of the channels added in Umbraco.
    /// </summary>
    [HttpGet("channels")]
    public IActionResult GetChannels() {
        return Ok(_dreamBrokerService.GetChannels());
    }

    /// <summary>
    /// Adds the channel with the specified <paramref name="channelId"/> and <paramref name="name"/> to Umbraco.
    /// </summary>
    /// <param name="channelId">The DreamBroker ID of the channel.</param>
    /// <param name="name">The name of the channel.</param>
    [HttpPost("channels")]
    public IActionResult AddChannel(string? channelId, string? name) {

        if (string.IsNullOrWhiteSpace(channelId)) return BadRequest("No channel ID specified.");
        if (string.IsNullOrWhiteSpace(name)) return BadRequest("No channel name specified.");

        // Don't add the same channel twice
        if (_dreamBrokerService.GetChannel(channelId) is { } existing) return Ok(existing);

        return Ok(_dreamBrokerService.AddChannel(channelId, name));

    }

    /// <summary>
    /// Deletes the channel with the specified <paramref name="channelId"/> from Umbraco.
    /// </summary>
    /// <param name="channelId">The DreamBroker ID or key of the channel.</param>
    [HttpDelete("channels/{channelId}")]
    public IActionResult DeleteChannel(string? channelId) {

        if (string.IsNullOrWhiteSpace(channelId)) return BadRequest("No channel ID specified.");

        DreamBrokerChannel? channel = _dreamBrokerService.GetChannel(channelId);
        if (channel == null) return NotFound("Channel not found.");

        _dreamBrokerService.DeleteChannel(channel);

        return Ok();

    }

    /// <summary>
    /// Returns a list of videos of the channels added in Umbraco.
    /// </summary>
    /// <param name="text">If specified, only videos matching this parameter will be returned.</param>
    [HttpGet("videos")]
    public IActionResult GetVideos(string? text = null) {

        List<object> channels = [];

        // Iterate through the channels added to Umbraco
        foreach (DreamBrokerChannel channel in _dreamBrokerService.GetChannels()) {

            IEnumerable<VideoItem> videos;

            try {

                // Get the videos of the channel
                videos = _dreamBrokerService
                    .GetChannelVideos(channel)
                    .Where(x => string.IsNullOrWhiteSpace(text) || x.Title.InvariantIndexOf(text) >= 0 || x.VideoId == text)
                    .ToArray();

            } catch (Exception ex) {

                // A single unreachable channel shouldn't take down the entire list
                _logger.LogError(ex, "Failed getting videos of DreamBroker channel with ID {ChannelId}.", channel.ChannelId);

                continue;

            }

            // Append the channel to the overall list
            channels.Add(new {
                channelId = channel.ChannelId,
                name = channel.Name,
                videos
            });

        }

        return Ok(new { channels });

    }

    /// <summary>
    /// Returns information about the video with the specified <paramref name="channelId"/> and <paramref name="videoId"/>.
    /// </summary>
    /// <param name="channelId">The ID of the channel.</param>
    /// <param name="videoId">The ID of the video.</param>
    [HttpGet("video")]
    public IActionResult GetVideo(string? channelId, string? videoId) {

        if (string.IsNullOrWhiteSpace(channelId)) return BadRequest("No channel ID specified.");
        if (string.IsNullOrWhiteSpace(videoId)) return BadRequest("No video ID specified.");

        // Get a reference to the channel (if stored in Umbraco)
        DreamBrokerChannel? channel = _dreamBrokerService.GetChannel(channelId);

        // If the channel isn't in Umbraco, we try to look up its name so we can suggest adding it
        DreamBrokerChannelDetails channelDetails = channel is null
            ? _dreamBrokerService.GetChannelName(channelId) is { } name
                ? new DreamBrokerChannelDetails(channelId, name, false)
                : new DreamBrokerChannelDetails(channelId)
            : new DreamBrokerChannelDetails(channel);

        VideoItem? video;

        try {

            // As DreamBroker doesn't really have an API, we get all the videos of the channel via their internal API,
            // and then pick the video with the matching ID
            video = _dreamBrokerService.GetChannelVideos(channelId).FirstOrDefault(x => x.VideoId == videoId);

        } catch (Exception ex) {

            _logger.LogError(ex, "Failed getting videos of DreamBroker channel with ID {ChannelId}.", channelId);

            return InternalServerError("Failed getting video information from DreamBroker.");

        }

        if (video == null) return NotFound("Video not found.");

        return Ok(new {
            channel = channelDetails,
            video
        });

    }

    #endregion

    #region Private methods

    /// <summary>
    /// Our models are annotated with Newtonsoft attributes, so they must not be serialized by the
    /// <c>System.Text.Json</c> serializer used by the management API by default.
    /// </summary>
    private static new NewtonsoftJsonResult Ok(object value) {
        return NewtonsoftJsonResult.Ok(value);
    }

    private static IActionResult InternalServerError(object value) {
        return new ObjectResult(value) {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }

    #endregion

}
