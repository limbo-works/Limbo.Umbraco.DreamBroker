// Holds the server variables of the package - see the "serverVariables" endpoint of DreamBrokerController.
export class DreamBrokerPackage {

    static _serverVariables = {};

    static set serverVariables(value) {
        this._serverVariables = value ?? {};
    }

    static get version() {
        return this._serverVariables["version"];
    }

    static get cacheBuster() {
        return this._serverVariables["cacheBuster"];
    }

}

export default DreamBrokerPackage;
