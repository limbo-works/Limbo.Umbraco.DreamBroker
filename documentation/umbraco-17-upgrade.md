# Umbraco 17 upgrade

Recap of the upgrade of **Limbo.Umbraco.DreamBroker** from Umbraco 13 to Umbraco 17, done on the `v17/dev` branch.

The package went from `13.0.3` to **`17.0.0-alpha000`**, and from **.NET 8 to .NET 10**. The whole backoffice layer was rewritten, because Umbraco 14 replaced the AngularJS backoffice with a Lit/web-component one and there is no compatibility shim.

The [`Limbo.Umbraco.TwentyThree`](https://github.com/limbo-works/Limbo.Umbraco.TwentyThree) `v17/dev` branch was used as the reference implementation, so this package follows the same conventions as its sibling video-picker packages.

## TL;DR

| | v13 | v17 |
|---|---|---|
| Target framework | `net8.0` | `net10.0` |
| Package version | `13.0.3` | `17.0.0-alpha000` |
| Umbraco references | `[13.0.0,13.999)` | `[17.0.0,17.9.9)` |
| Backoffice | AngularJS (`.html` views + controllers) | Lit / ESM web components |
| Manifest | `IManifestFilter` | `IPackageManifestReader` + `backofficeEntryPoint` |
| Backoffice API | `UmbracoAuthorizedApiController` | `ManagementApiControllerBase` |
| Styling | LESS compiled to CSS | `static styles` on each element |
| Localization | `Lang/*.xml` | `Localization/*.js` |
| Icons | `.svg` files | inline SVG strings |

**The saved property value did not change**, so existing content keeps working without a migration.

## Dependencies

```
Limbo.Umbraco.Video          13.0.0        -> 17.0.0-alpha001
Skybrud.Essentials.Http      1.2.1         -> 1.2.2
Skybrud.Essentials           (transitive)  -> 1.1.68     (now explicit)
Skybrud.Essentials.AspNetCore  -           -> 1.0.2      (new: NewtonsoftJsonResult)
Umbraco.Cms.Core             [13.0.0,…)    -> [17.0.0,17.9.9)
Umbraco.Cms.Web.Website      [13.0.0,…)    -> [17.0.0,17.9.9)
Umbraco.Cms.Web.Common         -           -> [17.0.0,17.9.9)   (new)
Umbraco.Cms.Api.Management     -           -> [17.0.0,17.9.9)   (new)
Umbraco.Cms.Web.BackOffice   [13.0.0,…)    -> removed
```

`Umbraco.Cms.Web.BackOffice` was dropped: it existed only for `UmbracoAuthorizedApiController`. `<LangVersion>12.0</LangVersion>` was removed so the project uses the SDK default for .NET 10, and `<NuGetAuditMode>direct</NuGetAuditMode>` was added so vulnerabilities in Umbraco's transitive dependencies don't fail our build.

## Server side changes

### Package manifest

`IManifestFilter` no longer exists. `Manifests/DreamBrokerManifestFilter.cs` was replaced by **`Manifests/DreamBrokerPackageManifestReader.cs`**, implementing `IPackageManifestReader`. The old manifest listed AngularJS scripts and a stylesheet; the new one declares:

- one `backofficeEntryPoint` extension pointing at `EntryPoint.js`, and
- an **importmap** mapping `@limbo/dreambroker/*` bare specifiers onto the static web asset URLs, so the client modules can import each other by name.

There is deliberately no `umbraco-package.json` — everything is served from C#, which keeps the cache buster tied to the assembly version.

### Backoffice API

`Controllers/DreamBrokerController.cs` was rewritten from `UmbracoAuthorizedApiController` + `[PluginController("Limbo")]` to a versioned Management API controller. Routes moved:

```
/umbraco/backoffice/Limbo/DreamBroker/{Action}      (old)
/umbraco/management/api/v1/limbo/dreambroker/…      (new)
```

The endpoints were also made properly RESTful while they were being rewritten:

| Old | New |
|---|---|
| `GET GetChannels` | `GET channels` |
| `GET AddChannel?channelId=&name=` | `POST channels?channelId=&name=` |
| `GET DeleteChannel?channelId=` | `DELETE channels/{channelId}` |
| `GET GetVideos?text=` | `GET videos?text=` |
| `GET GetVideo?channelId=&videoId=` | `GET video?channelId=&videoId=` |
| — | `GET serverVariables` (new — version + cache buster) |

Two things worth knowing:

- The management API serializes with `System.Text.Json`, but all our models are annotated with Newtonsoft `[JsonProperty]`. Responses therefore go through **`NewtonsoftJsonResult`** (there's a private `Ok` overload in the controller that does this). Forget it and property names come out PascalCased.
- `Api/DreamBrokerApiConstants`, `Api/DreamBrokerSecurityFilter` and `Api/DreamBrokerSwaggerGenOptions` were added so the package gets its own OpenAPI document with the backoffice bearer-token requirement.

`AddChannel` is now idempotent (it returns the existing channel instead of adding a duplicate), and `GetVideos` no longer lets one unreachable channel take down the whole list — failures are logged per channel and skipped.

### Property editor

The `[DataEditor]` attribute lost its name, view, icon and group parameters — all of that is now declared client side:

```csharp
// before
[DataEditor(EditorAlias, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Limbo", Icon = EditorIcon)]
// after
[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
```

`IEditorConfigurationParser` was removed from Umbraco, so `ConfigurationEditor<T>` now takes only `IIOHelper`. The `GetValueEditor` override that appended `?v=…` to the view path is gone with the view itself. `[ConfigurationField]` now only takes an alias; labels, descriptions and editors for each field live in the `settings` of the client-side `propertyEditorSchema` manifest.

### Other

- `DreamBrokerService.GetChannelName(string)` is new — the channel-page scrape moved out of the controller so all HTTP calls live in the service.
- `DreamBrokerPackage` now derives `InformationalVersion` from `ReflectionUtils` instead of `FileVersionInfo`, and exposes a new `CacheBuster` property (MD5 of the informational version) used for cache busting the client assets.
- `Skybrud.Essentials.Json.Extensions` → `Skybrud.Essentials.Json.Newtonsoft.Extensions`, and the obsolete `TimeSpanSecondsConverter` → `TimeSpanConverter`. The latter defaults to `TimeSpanFormat.Seconds`, i.e. **the same wire format**, so stored durations are unaffected.

## Client side changes

Everything under `wwwroot/` was replaced. The new code is plain ESM + Lit — **no TypeScript, no bundler, no `node_modules`**, matching the sibling packages.

| Removed | Replaced by |
|---|---|
| `Scripts/Services/DreamBrokerService.js` | `Service.js` (+ `Auth.js`, `Package.js`) |
| `Scripts/Controllers/Video.js` + `Views/Video.html` | `Elements/Video.js` |
| `Scripts/Controllers/VideoOverlay.js` + `Views/VideoOverlay.html` | `Modals/SelectVideo.js` |
| `Scripts/Controllers/SuggestChannelOverlay.js` + `Views/SuggestChannelOverlay.html` | `Modals/SuggestChannel.js` |
| `Styles/Default.less` + `Default.css` + `compilerconfig.json` | `static styles` on each element |
| `Lang/en-US.xml`, `Lang/da-DK.xml` | `Localization/en-US.js`, `Localization/da-DK.js` |
| `BackOffice/Icons/*.svg` | `Icons.js` + `Icons/*.js` (inline SVG strings) |
| — | `EntryPoint.js`, `Modals/Tokens.js` |

Notes:

- **Auth.** The old service relied on ambient cookie auth and `Umbraco.Sys.ServerVariables`. Requests now need an explicit bearer token, which `EntryPoint.js` pulls off `UMB_AUTH_CONTEXT` and hands to `Service.js`.
- **Localization** moved from the `dreambroker` XML area to a `limboDreamBroker` JS area (`limboDreamBroker_*`). Duration formatting is *not* reimplemented — it reuses `<limbo-video-duration>` and the `limboVideo_*` terms from `Limbo.Umbraco.Video`, so the hand-rolled Danish-only `getDuration()` helper is gone and durations are now localized properly.
- **Search actually works now.** The old overlay fetched every video of every channel and filtered in the browser; the `text` parameter the controller had always accepted was never used. The new modal debounces and passes the search to the server.
- **Icons** are brand-coloured by baking the fills (`#1b191a` / `#89339e`) into `Icons/DreamBrokerAlt.js`; they used to come from LESS rules targeting `path.d` / `path.b`, which can't work now that icons are inlined SVG. `Icons/DreamBroker.js` uses `currentColor` so it tints with the backoffice theme.

## Bugs found and fixed during review

An extension review against the real Umbraco 17.5 client sources turned up several things that would have broken at
runtime. They're listed here because most of them are traps any package making this jump will hit.

1. **Umbraco 17 authenticates the backoffice with a cookie, not a bearer token.** `UmbAuthContext.getLatestToken()`
   literally returns the string `"[redacted]"` — the real credential is an httpOnly cookie. A `fetch` therefore has to
   send `credentials` (and use the configuration's `base`); an `Authorization` header alone carries nothing. `Auth.js`
   now stores the whole `getOpenApiConfiguration()` result and `Service.js` uses it.
2. **Extensions must be registered synchronously in the entry point.** The first version awaited the
   `serverVariables` request before registering. If a content node resolves its property editor UI in that window,
   `umb-property` permanently swaps to the "missing property editor" UI and never re-checks. The cache buster is now
   read off the entry point's own URL (`import.meta.url`), which the manifest already stamps, so nothing needs to be
   awaited before registering.
3. **A value with a `source` but no `details` crashed rendering.** `DreamBrokerVideoValue` used a null-forgiving `!` on
   the details lookup and then passed the result to `DreamBrokerEmbed`, which dereferences it — so saving a property
   while a half-typed URL was in the box produced a `NullReferenceException` on the front end. This was latent in v13
   too, but the debounced editor makes it much easier to hit. `Parse` now returns `null` when there are no usable
   details, and `DreamBrokerVideoDetails.Parse` rejects details without a channel and video ID (which would otherwise
   throw in `DreamBrokerThumbnail.Create`).
4. **`application/problem+json` was never parsed.** `"application/problem+json".includes("application/json")` is
   `false`, so every API error collapsed to a generic message. Now tests for `json`.
5. **XSS in the suggest-channel dialog.** The channel name is scraped out of DreamBroker's HTML with a regex, and the
   dialog interpolated it into a localization string that was then rendered with `unsafeHTML`. The dialog now renders
   the video and channel names as plain text bindings, which Lit escapes.
6. **`[Authorize]` was too narrow.** `SectionAccessContent` would have locked out editors with only Media or Members
   access, even though the data type can be used on media and member types. Now `BackOfficeAccess`.
7. **The Swagger security filter never applied.** `BackOfficeSecurityRequirementsOperationFilterBase.ApiName` is
   compared against the Swagger **document name**, so it must return the alias we pass to `SwaggerDoc`
   (`limbo-dreambroker-v1`) — not the human-readable title. With the title, all operations were emitted with no
   security requirement and no `Backoffice-User` scheme. Note that this only ever affected the Swagger document, never
   actual enforcement, which comes from `[Authorize]`. **The same mistake is present in the `Limbo.Umbraco.TwentyThree`
   `v17/dev` branch** that was used as the reference, so it's worth fixing there too.

Also tightened while in there: the mandatory validator is now actually registered (the element declared `mandatory` but
never enforced it), `_loading`/`_error`/etc. are reactive Lit state instead of hand-rolled `requestUpdate()` calls, and
the empty value is `undefined` rather than `null`.

## Breaking changes for consumers

1. **`DreamBrokerChannelDetails` moved namespace** — from `Limbo.Umbraco.DreamBroker.Controllers` to `Limbo.Umbraco.DreamBroker.Models.Channels` (it was declared inline in the controller file).
2. **`DreamBrokerPackage` is now a `static class`** (it was a non-static class with only static members).
3. **`DreamBrokerVideoEditor` constructor signature changed** — `(IDataValueEditorFactory, IIOHelper)`; `IEditorConfigurationParser` is gone. `EditorView` was removed and `EditorIcon` is now `"limbo-dreambroker-alt"` (no `color-limbo` CSS class). A new `EditorUiAlias` constant was added.
4. **`DreamBrokerVideoConfigurationEditor` constructor signature changed** — `(IIOHelper)` only.
5. **`IVideoDetails.Thumbnails` / `.Files` are `IReadOnlyList<T>`** rather than `IEnumerable<T>`, following `Limbo.Umbraco.Video` 17. `DreamBrokerVideoDetails` was updated to match.
6. **The `hideLabel` data type setting no longer does anything.** Umbraco 17 gives a property editor UI no way to hide the label of the property it is rendered in — there is no such hook anywhere in the backoffice client. `DreamBrokerVideoConfiguration.HideLabel` is kept and marked `[Obsolete]` so configuration saved by older versions still deserializes, but it is no longer editable and has no effect.
7. **Backoffice API routes changed** (see the table above). This only matters for anything calling those endpoints directly — they are backoffice-internal.

## Verification done

- `dotnet build` — succeeds with **zero errors and zero warnings**.
- `dotnet pack` — produces `Limbo.Umbraco.DreamBroker.17.0.0-alpha000.nupkg`; verified it contains `lib/net10.0` plus all 13 client modules under `staticwebassets/`, and that every Umbraco dependency is pinned to `[17.0.0, 17.9.9)`.
- All client modules pass an ESM syntax check (`node --input-type=module --check`).
- Every extension manifest was validated against `umbraco-package-schema.json` from the real `@umbraco-cms/backoffice`
  17.5.3 package (required `meta` keys included), and every `@umbraco-cms/backoffice/*` import was checked to exist
  rather than assumed. The `%0%` placeholder syntax and the `general_*` localization keys used were likewise confirmed
  against the shipped sources.
- **Installed into a real Umbraco 17.5.3 site** (SQLite, unattended install) and booted:
  - the site starts with no exceptions in the log;
  - all 13 client modules are served from `/App_Plugins/Limbo.Umbraco.DreamBroker/`;
  - `Limbo.Umbraco.Video`'s `Elements/Duration.js`, which our element imports, resolves too;
  - all 6 API endpoints are routed at the expected paths and return **401 when unauthenticated**;
  - the package's own Swagger document is generated with the correct title and the `Backoffice-User` security
    requirement on every operation.

**Not done:** the backoffice UI has not been driven in a browser — booting the site exercises the server side, the
manifest and asset delivery, but not the Lit components themselves. There is no test project in this repository, so
there's no automated coverage of runtime behaviour either. Before this leaves alpha, someone should log in and check:
pasting a URL, picking a video from the modal, the suggest-channel prompt, clearing a value, mandatory validation, and
that a value saved by v13 still renders. Those flows need a real DreamBroker channel, which this verification did not
have.
