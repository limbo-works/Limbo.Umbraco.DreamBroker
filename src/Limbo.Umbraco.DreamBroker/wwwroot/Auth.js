// Holds the OpenAPI configuration of the current backoffice session, as handed to us by EntryPoint.js.
//
// Umbraco 17 authenticates the backoffice with an httpOnly cookie - "getLatestToken()" deliberately returns the
// string "[redacted]" - so the important part of this configuration is "credentials", not the token. See
// UmbAuthContext.getOpenApiConfiguration().
let resolveConfiguration;

const whenConfigured = new Promise((resolve) => {
    resolveConfiguration = resolve;
});

export const DreamBrokerAuth = {

    /** The OpenAPI configuration, once EntryPoint.js has resolved the auth context. */
    CONFIG: undefined,

    setConfiguration(config) {
        this.CONFIG = config;
        resolveConfiguration(config);
    },

    /** Resolves once the auth context is available - extensions may be registered before that happens. */
    async configuration() {
        return this.CONFIG ?? (await whenConfigured);
    }

};

export default DreamBrokerAuth;
