const basicAuth = require('basic-auth');


module.exports = ({app}) => {

    app.all('/test/hello', function (req, res) {
        console.warn("hello world");
        res.send('Hello World!');
    });

    app.all('/test/headers', function (req, res) {
        if(req.header('X-BaconPancakes') != null){
            res.set('X-BaconPancakes', "Makin' Bacon Pancakes");
            return res.send("Makin' Bacon Pancakes");
        }
        return res.status(400).send('NO BACON PANCAKES DETECTED');
    });

    app.all('/test/serverError', function(req, res) {
        return res.status(500).send("oh no!");
    });

    app.all('/test/clientError', function(req, res) {
        return res.status(400).send("oh no!");
    });

    app.all('/test/unauthorized', function(req, res) {
        return res.status(401).send("oh no!");
    });

    app.all('/test/forbidden', function(req, res) {
        return res.status(403).send("oh no!");
    });

    app.all('/test/timeout', function(req, res) {
        return;
    });

    // Here we go, vertigo
    // Video vertigo
    // Test for echo
    app.all(`/test/echo`, function(req, res){
        let response = JSON.stringify(req.body);
        console.warn(response);
        res.header("Content-Type", "application/json");
        return res.status(200).send(response);
    });

    app.delete('/test/delete', function(req, res){
        return res.status(200).send("OK");
    });

    app.get('/test/auth/basic', function(req, res){

        let credentials = basicAuth(req);

        if (!credentials) {
            return res.status(401).send("No Credential Provided");
        }

        const username = credentials.name;
        const password = credentials.pass;

        return res.status(200).send("Hello World!");
    });

};