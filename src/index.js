import React from "react";
import ReactDOM from "react-dom";
import bridge from "@vkontakte/vk-bridge";
import App from "./App";
import * as eruda from 'eruda';
import * as erudaCode from 'eruda-code';
import * as erudaDom from 'eruda-dom';


eruda.init();
eruda.add(erudaCode);
eruda.add(erudaDom);

ReactDOM.render(
    <App />
  , document.getElementById("root"));

