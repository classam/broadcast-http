const WebSocket = require('ws');                                // pipeline pipeline pipeline
const uuid = require('uuid/v4');


const CONNECTION_COUNT_INTERVAL = 5000;
const SOCKET_INTERVAL_MILLISECONDS = 500;
const PING_INTERVAL_MILLISECONDS = 30000;

module.exports = ({httpServer}) => {

    /*
        1. make sure that the user is authed properly
        2. send them every message in their token queue
        3. send them every message in their message queue
     */

    let activeConnections = new Set();

    setInterval(async()=>{
        if(activeConnections.size > 0){
            console.log("Pipeline is holding on to " + activeConnections.size + " connections");
        }
        if(activeConnections.size > 1000){
            console.error("VERY BAD ERROR: NODE IS HOLDING ON TO >1000 CONNECTIONS");
        }
        if(activeConnections.size > 5000){
            console.error("CRITICAL ERROR: NODE IS HOLDING ON TO >5000 CONNECTIONS");
            process.exit(1);
        }
        activeConnections = new Set();
    }, CONNECTION_COUNT_INTERVAL);

    const wss = new WebSocket.Server({
        server: httpServer,
    });

    const establishConnection = async ({req}) => {
        return new Promise((resolve, reject)=>{
            let session = {};
            return resolve(session);
        });
    };

    httpServer.on('upgrade', async (req, socket, head) => {
        console.log("upgrading connection");

        const session = await establishConnection({req}).catch((err)=>{
            if(err != null){
                console.log("Upgrading Connection Error: ", err);
                socket.destroy();
            }
        });

        console.log("Successfully upgraded to websocket connection!");
    });

    wss.on('connection', async function connection(ws, req){

        console.log("starting connection");

        let updateInterval = null;
        let pingInterval = null;

        let connectionId = uuid();

        const close = () => {
            try{
                clearInterval(updateInterval);
                clearInterval(pingInterval);
            }
            catch(err){
                console.error(err);
            }

            console.log("conn=" + connectionId + " Closing connection!");
            return ws.terminate();
        };

        const session = await establishConnection({req}).catch((err)=>{
            if(err != null){
                console.log("Establish Connection Error: ", err);
                ws.send(JSON.stringify(err), (err)=>{
                    if(err != null){
                        console.error("Pipeline error: ", err);
                    }
                });

                close();
            }
        });

        console.log("Successfully started connection!");

        ws.on('message', function incoming(message){
            console.log("Received message:", message);
            ws.send(message, (err)=>{
                console.error(err);
            })
        });

        let failed = false;

        ws.send("hi");

        updateInterval = setInterval(async()=>{
            //req.console.log("maintaining connection");

            activeConnections.add(connectionId);

            /*
            let type = "hi";
            let content = "hi";

            ws.send(JSON.stringify({
                type: type,
                content: content,
            }), (err) => {
                if(err != null){
                    console.warn(`Failed to send ${type} - ${content} -  ${err}`);
                    failed = true;
                }
            });
             */

            if(failed){
                console.warn(`Closing connection due to failed send operation`);

                close();
            }
        }, SOCKET_INTERVAL_MILLISECONDS);

        var isAlive = true;

        ws.on('pong', ()=>{
            isAlive = true;
        });

        pingInterval = setInterval(async()=>{
            //req.console.log("doing a ping");
            if(isAlive === false) {
                console.warn("Failed to maintain connection with pings; Terminating.")
                close();
            }

            isAlive = false;
            ws.ping(() => {});
        }, PING_INTERVAL_MILLISECONDS);

        ws.on('close', function endConnection(){
            console.log("closing connection");
            close();
        });
    })

}
