
module.exports = ({app}) => {

    app.get('/test/hello', function (req, res) {
        console.warn("hello world");
        res.send('Hello World!');
    });

    app.get('/test/headers', function (req, res) {
        if(req.header('X-BaconPancakes') != null){
            return res.send("Makin' Bacon Pancakes");

        }
        return res.status(400).send('NO BACON PANCAKES DETECTED');
    });

    app.get('/test/serverError', function(req, res) {
        return res.status(500).send("oh no!");
    });

    app.get('/test/clientError', function(req, res) {
        return res.status(400).send("oh no!");
    });

    app.get('/test/unauthorized', function(req, res) {
        return res.status(401).send("oh no!");
    });

    app.get('/test/forbidden', function(req, res) {
        return res.status(403).send("oh no!");
    });

    app.get('/test/timeout', function(req, res) {
        return;
    });

    // Here we go, vertigo
    // Video vertigo
    // Test for echo
    app.post(`/test/echo`, function(req, res){
        let response = JSON.stringify(req.body);
        console.warn(response);
        res.header("Content-Type", "application/json");
        return res.status(200).send(response);
    });

    app.put(`/test/echo`, function(req, res){
        let response = JSON.stringify(req.body);
        console.warn(response);
        res.header("Content-Type", "application/json");
        return res.status(200).send(response);
    });

    app.patch(`/test/echo`, function(req, res){
        let response = JSON.stringify(req.body);
        console.warn(response);
        res.header("Content-Type", "application/json");
        return res.status(200).send(response);
    });

    app.delete('/test/delete', function(req, res){
        return res.status(200).send("OK");
    });

};