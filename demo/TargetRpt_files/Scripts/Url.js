var URL = (function (a) {
    return {
        // create a querystring from a params object
        serialize: function (params) {
            var key, query = [];
            for (key in params) {
                query.push(encodeURIComponent(key) + "=" + encodeURIComponent(params[key]));
            }
            return query.join('&');
        },

        // create a params object from a querystring
        unserialize: function (query) {
            var pair, params = {};
            query = query.replace(/^\?/, '').split(/&/);
            for (pair in query) {
                pair = query[pair].split('=');
                params[decodeURIComponent(pair[0])] = decodeURIComponent(pair[1]);
            }
            return params;
        },

        parse: function (url) {
            a.href = url;
            return {
                // native anchor properties
                hash: a.hash,
                host: a.host,
                hostname: a.hostname,
                href: url,
                pathname: a.pathname,
                port: a.port,
                protocol: a.protocol,
                search: a.search,
                // added properties
                file: a.pathname.split('/').pop(),
                params: URL.unserialize(a.search)
            };
        }
    };
}(document.createElement('a')));
