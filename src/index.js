import { createServer } from "https";
import { Server } from "socket.io";

import fs from "fs";
import express from "express";
import cors from 'cors';
import sequelize from "./sequelize.js";
import axios from "axios";


const Coor = sequelize.model.Coor;

const options = {
  key:    fs.readFileSync("/root/fff.key"),
  cert:   fs.readFileSync("/root/fff.crt")
}

var app = express();

app.get("/", (req, res) => {
  res.send("api");
})

app.get("/user", (req, res) => {
  console.log("ok");
  axios.get("http://back.com.xsph.ru/api/user").then((data) => {
      res.send(data.data);
  })
})


const server = createServer(options, app);
const io = new Server(server, {
});



io.on("connection", (socket) => {
  socket.emit("eeee")

  socket.on("geo", (data) => {
    const { latitude, longitude } = data.coords;
    //Coor.create({ user_id: data.user_id, lat:latitude, lon:longitude }).then(() => {
      console.log("updated_geo");
    //
  })

  // socket.on("disconnect", () => {
  //     console.log("disconnect");
  // })

});



server.listen(3000, () => {
  console.log("server started on port 3000");
})





