<?php
header('Access-Control-Allow-Origin: *');
$file = json_decode(file_get_contents("php://input"), true);

switch($file['type']) {
    case "user":
        echo post_api("user", $file['obj']);
    break;

    case "user2":
        echo get_api("user", null);
    break;

    case "quest":
        echo get_api("quests", null);
    break;

    case "user-last-update":
        $ch = curl_init();
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'PUT');
        curl_setopt($ch, CURLOPT_URL,"http://back.com.xsph.ru/api/user-last-update/".$file['obj']['user_id']);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $file['obj']);
        $ser = curl_exec($ch);
        curl_close($ch);
        echo $ser;
    break;

    case "answer-user":
        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL,"http://back.com.xsph.ru/api/".$file['type']);
        curl_setopt($ch, CURLOPT_POST, 1);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $file['obj']);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        echo curl_exec($ch);
        curl_close ($ch);
        
        exit;
    break;

    case "coor-users":
        echo post_api("coor-users", $file['obj']);
    break;

    default:
        echo "o";
}



function post_api($method, $obj) {
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL,"http://back.com.xsph.ru/api/".$method);
    curl_setopt($ch, CURLOPT_POST, 1);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $obj);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    echo curl_exec($ch);
    curl_close ($ch);
    return $ser;
    
};

function get_api($method, $obj) {
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL,"http://back.com.xsph.ru/api/".$method);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    $ser = curl_exec($ch);

    curl_close($ch);
    echo $ser;
};

function put_api($method, $obj) {
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'PUT');
    curl_setopt($ch, CURLOPT_URL,"http://back.com.xsph.ru/api/".$method);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, $obj);
    $ser = curl_exec($ch);
    curl_close($ch);
    echo $ser;
};

?>