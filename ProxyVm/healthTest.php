<?php
error_reporting(E_ALL);
ini_set('display_errors', 1);

$url = 'http://krak.dk/';
$proxy = '127.0.0.1:21777';

isPageAccessible($url, $proxy);

function isPageAccessible($url, $proxy)
{
        if(getPageHtml($url, $proxy) != '')
        {
                echo 'good :)';
        }
        else
        {
                echo 'bad :(';
                header($_SERVER['SERVER_PROTOCOL'] . ' 500 Internal Server Error', true, 500);
        }
}

function getPageHtml($url, $proxy)
{
        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_PROXY, $proxy);

        //follow any "Location: " header
        curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);

        //return the transfer as a string of the return value of curl_exec() instead of outputting it out directly.
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);

        //the number of seconds to wait while trying to connect. Use 0 to wait indefinitely.
        curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 10);

        //the maximum number of seconds to allow cURL functions to execute.
        curl_setopt($ch, CURLOPT_TIMEOUT, 5);

        //load html
        $html = curl_exec($ch);

        //check http status
        $info = curl_getinfo($ch);

        if($info['http_code'] != '200')
        {
                $html = '';
        }

        curl_close($ch);

        return $html;
}

?>
