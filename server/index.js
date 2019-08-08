const express = require('express');
const cookieParser = require('cookie-parser');
const bodyParser = require('body-parser');
const responseTime = require('response-time');

const testRoute = require('./routes/test');
const pipeline = require('./pipeline');

// SERVER VARS
const port = process.env.BROADCAST_PORT || '40000';

// Create a new express application instance
const app = express();

const main = async () => {

    // parse application/x-www-form-urlencoded
    app.use(bodyParser.urlencoded({ extended: false }));

    // parse application/json
    app.use(bodyParser.json());

    // log every URL we hit, with timings
    app.use(responseTime((req, res, time) => {
        time = Math.ceil(time);
        console.log(`${req.method} ${req.url} ${time}ms`)
    }));

    //
    let httpServer = require('http').createServer(app);
    let routeArgs = {
        app: app,
        httpServer: httpServer,
    };

    app.use(cookieParser())

    app.get('/', function (req, res) {
        res.send('Hello World!');
    });

    testRoute(routeArgs);
    pipeline(routeArgs);

    httpServer.listen(port, function() {
        console.log(`Broadcast Text Server running on port ${port}`);
    });
};

main().catch((err)=>{
    console.error(`ERROR 1: Critical, unrecoverable error`, err);
});

