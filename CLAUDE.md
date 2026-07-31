# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`Limbo.Umbraco.DreamBroker` is a single-project NuGet package for Umbraco: a property editor that lets an editor paste a DreamBroker video URL (or pick a video from a known channel) and exposes the stored value as strongly-typed C# on the published content.

DreamBroker has **no public API**. Everything goes through undocumented endpoints on `dreambroker.com` that expose already-public data, so there is no authentication anywhere:

- `https://www.dreambroker.com/channel/v2/{channelId}/searchresources?limit=1000&offset=0` — all resources of a channel; the code filters `type == "VIDEO"`.
- `https://dreambroker.com/channel/oembed?url=…&format=json` — oEmbed details.
- `https://dreambroker.com/channel/{channelId}` — scraped with a regex (`channelTitle: '(.+?)',`) to recover a channel name (`DreamBrokerService.GetChannelName`).

Because there is no "get one video" endpoint, looking up a single video means fetching **all** videos of the channel and filtering client-side (`DreamBrokerService.GetIntermediaryVideoValue`, `DreamBrokerController.GetVideo`). Keep that in mind before adding call sites.

## Branches & versions

One branch per Umbraco major, and the default branch is **not** `main`:

| Branch | Umbraco | TFM |
|---|---|---|
| `v17/dev` (current) | 17 | net10.0 |
| `v13/main` (repo default) | 13 | net8.0 |
| `v2/main` | 10–12 | |
| `v1/main` | 9 | |

The package version lives in `<VersionPrefix>` in the `.csproj` (currently `17.0.0-alpha000`) and is duplicated in the README install snippets — update both when releasing. Umbraco package references are pinned to `[17.0.0,17.9.9)`.

`documentation/umbraco-17-upgrade.md` records what the v13 → v17 upgrade changed, including the breaking changes to the public C# API. Update it if you change any of that surface.

## Build & release

No test project exists; there is nothing to run tests with. There is also **no npm build** — the client-side code is hand-written ESM, shipped as-is.

```bash
dotnet build src/Limbo.Umbraco.DreamBroker            # plain build
./debug.bat                                            # Debug rebuild + pack -> c:\nuget\Umbraco17 (Windows paths)
./release.bat                                          # Release rebuild + pack -> releases/nuget
```

`debug.bat` appends a `build{yyyyMMddHHmm}` `VersionSuffix` in Debug so local packages always sort newer.

The build must stay **warning-free** — it currently is. To sanity-check the client-side modules without an Umbraco instance:

```bash
cd src/Limbo.Umbraco.DreamBroker/wwwroot
for f in $(find . -name "*.js"); do node --input-type=module --check < "$f" || echo "FAIL $f"; done
```

## Architecture

Backend (`src/Limbo.Umbraco.DreamBroker/`):

- `Composers/DreamBrokerComposer` — the only DI wiring: registers `DreamBrokerService` (transient), the `IPackageManifestReader`, and the Swagger options.
- `Manifests/DreamBrokerPackageManifestReader` — serves the `PackageManifest` from C#. There is deliberately **no `umbraco-package.json`**. It declares one `backofficeEntryPoint` (`wwwroot/EntryPoint.js`) plus an **importmap** mapping `@limbo/dreambroker/*` bare specifiers to the static web asset URLs. **A new module under `wwwroot/` that other modules import must be added to the importmap here**, or the import will fail at runtime.
- `Api/` — constants, the Swagger `OperationFilter` and `SwaggerGenOptions` for the package's own OpenAPI document.
- `Services/DreamBrokerService` — all HTTP calls (via `Skybrud.Essentials.Http`) plus channel persistence. Methods are `virtual` so consumers can subclass.
- `Controllers/DreamBrokerController` — a `ManagementApiControllerBase` routed via `[VersionedApiBackOfficeRoute("limbo/dreambroker")]`, so endpoints live at `/umbraco/management/api/v1/limbo/dreambroker/…`. **Actions must return `NewtonsoftJsonResult`** (there is a private `Ok` overload that does this) because the models are annotated with Newtonsoft attributes while the management API defaults to `System.Text.Json`.
- `PropertyEditors/` — `DreamBrokerVideoEditor` (alias `Limbo.Umbraco.DreamBroker`, JSON value type), its configuration/configuration-editor pair, and `DreamBrokerVideoValueConverter`. The `[DataEditor]` attribute carries only the alias and value type; name, icon, group and the settings UI are declared client-side in `EntryPoint.js`.
- `DreamBrokerUtils` — static facade that resolves `DreamBrokerService` off `StaticServiceProvider.Instance`, for callers without DI (e.g. import scripts).

Channels are stored in Umbraco's **key/value store**, not a custom table: `IKeyValueService` keys of the form `Limbo.Umbraco.DreamBroker.Channels.{Guid}` holding the JSON-serialized `DreamBrokerChannel`. Deleting a channel writes `null` rather than removing the row. There is no channel-management UI — when an editor pastes a URL for an unknown channel, the frontend offers to add it (the `SuggestChannel` modal).

### Value model

Two parallel shapes, both serializing to the same JSON — `{ source, details: { videoId, channelId, title, duration } }`:

- **Intermediary** (`Models/Videos/Intermediary/`) — what gets *written*. `DreamBrokerIntermediaryVideoValue` serialized to JSON **is** the database value; used when importing/creating values from code. The property editor UI writes the same shape.
- **Read** (`Models/Videos/`) — `DreamBrokerVideoValue` parsed back out by the value converter, implementing `IVideoValue` from the `Limbo.Umbraco.Video` dependency (with `DreamBrokerVideoProvider`/`Details`/`Embed`/`Thumbnail` implementing that package's interfaces). This is the integration point that lets DreamBroker videos be consumed alongside other video providers.

`duration` is written in **seconds** (`TimeSpanConverter` defaults to `TimeSpanFormat.Seconds`) even though `VideoItem` parses it from the DreamBroker API as milliseconds. `DreamBrokerVideoDetails` reads it back as seconds — don't "fix" one side in isolation.

`DreamBrokerVideoValue` reads legacy property names as fallbacks (`url` → `source`, `video` → `details`). Preserve that when touching parsing. **The stored value shape must not change** — it is the compatibility contract with content saved by v13 and earlier.

### Conventions in the C# code

- Models are immutable, parsed from `JObject` via a private ctor plus an `internal static Parse(JObject?)` annotated `[return: NotNullIfNotNull(nameof(json))]`; several derive from `Skybrud.Essentials`' `JsonObjectBase`, which keeps the source `JObject` accessible.
- **Newtonsoft.Json** throughout (`[JsonProperty]`), not `System.Text.Json` — `Limbo.Umbraco.Video` still exposes Newtonsoft types, so this isn't optional.
- Use the `Skybrud.Essentials.Json.Newtonsoft.*` namespaces; the older `Skybrud.Essentials.Json.Extensions` / `Skybrud.Essentials.Json.Converters.Time` equivalents are obsolete and will warn.
- Nullable enabled; file-scoped namespaces; `#pragma warning disable 1591`/`CS1591` at the top of files whose public members are intentionally undocumented. Public API otherwise carries full XML docs — the build emits an XML documentation file, so keep it that way.
- Removed/renamed public members get an `[Obsolete]` forwarder rather than being deleted (see `GetDreamBrokerVideoValueFromSource`, `GetIntermediaryVideoValueFromSource`).
- Custom exceptions all derive from `DreamBrokerException` in `Exceptions/`.
- `src/.editorconfig` is authoritative: 4-space indent, **CRLF**, no final newline, `dotnet_sort_system_directives_first`, and opening braces on the same line (K&R) as seen across the codebase.

### Frontend

Plain **ESM + Lit** under `wwwroot/`, deployed to `/App_Plugins/Limbo.Umbraco.DreamBroker/` as static web assets (`StaticWebAssetBasePath`). No TypeScript, no bundler, no `node_modules` — match that style rather than introducing a build step.

- `EntryPoint.js` — the single registration point. Grabs the access token off `UMB_AUTH_CONTEXT`, fetches server variables, then registers localizations, icons, the `propertyEditorSchema`, the `propertyEditorUi` and the two modals. Registration is guarded by a flag because `consumeContext` can fire more than once.
- `Service.js` / `Auth.js` / `Package.js` — the API client, the token holder, and the server-variable holder.
- `Elements/Video.js` — the property editor UI (`limbo-dreambroker-video`).
- `Modals/Tokens.js` + `Modals/{SelectVideo,SuggestChannel}.js`.
- `Localization/{en-US,da-DK}.js` — the `limboDreamBroker` area, referenced as `limboDreamBroker_*`.
- `Icons.js` + `Icons/*.js` — icons are inline SVG **strings**, not `.svg` files.

Assets are cache-busted with `?v={cacheBuster}`, where `cacheBuster` is an MD5 of the informational version (`DreamBrokerPackage.CacheBuster`), served to the client via the `serverVariables` endpoint.

Duration rendering reuses the `<limbo-video-duration>` element from `Limbo.Umbraco.Video` (`import "@limbo/video/elements/duration"`), which also owns the `limboVideo_*` localization terms — don't reimplement duration formatting here.

The old `hideLabel` data type setting is intentionally not exposed any more: Umbraco 17 has no way for a property editor UI to hide its own label. The C# property is kept (and `[Obsolete]`) purely so existing stored configuration still deserializes.
