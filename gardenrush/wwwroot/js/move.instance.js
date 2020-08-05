﻿function tileAnimateImage(idFrom, idTo, imageSource)
{
    mover.initMove();

    moveObject.constructor.time = 500;     // 500ms total move time

    let fromObj = document.getElementById(idFrom);
    let toObj = document.getElementById(idTo);

    mover.setImage(imageSource);

    let boardObj = document.getElementById("board");
    let boardRect = boardObj.getBoundingClientRect();
    let fromRect = fromObj.getBoundingClientRect();
    let toRect = toObj.getBoundingClientRect();

    let relStartX = fromRect.left - boardRect.left;
    let relStartY = fromRect.top - boardRect.top;
    let relEndX = toRect.left - boardRect.left;
    let relEndY = toRect.top - boardRect.top;

    mover.start(parseInt(relStartX), parseInt(relStartY), parseInt(relEndX), parseInt(relEndY));
}

function tileAnimate(idFrom, idTo) {
    mover.initMove();
    let fromObj = document.getElementById(idFrom);
    let toObj = document.getElementById(idTo);

    let backgroundImage = fromObj.style.backgroundImage;
    fromObj.style.backgroundImage = "url()";

    rawImage = backgroundImage.substring(5, backgroundImage.length - 2);
    mover.setImage(rawImage);

    let boardObj = document.getElementById("board");
    let boardRect = boardObj.getBoundingClientRect();
    fromRect = fromObj.getBoundingClientRect();
    toRect = toObj.getBoundingClientRect();

    let relStartX = fromRect.left - boardRect.left;
    let relStartY = fromRect.top - boardRect.top;
    let relEndX = toRect.left - boardRect.left;
    let relEndY = toRect.top - boardRect.top;

    mover.start(parseInt(relStartX), parseInt(relStartY), parseInt(relEndX), parseInt(relEndY));
}

var mover = new moveObject();
var placesound = new soundplayer("/audio/place.mp3");
var pickupsound = new soundplayer("/audio/pickup_vol1.mp3");