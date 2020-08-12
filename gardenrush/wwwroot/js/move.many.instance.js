


function receiveArray(array,idTo)
{
    for (i = 0; i < array.length; i++)
    {   
        let fromObj = document.getElementById(array[i]);
        fromObj.innerHTML = "Got You";
    }
    let fromObj = document.getElementById(idTo);
    fromObj.innerHTML = "Got You";

}


function fillImageArray()
        {
            for(i=1;i<=10;i++)
            {
                let imageElement=document.getElementById("image"+i);
                imageArray.push(imageElement);
            }
        }
            
        function doAnimateArray(fromArray)
        {
            fillImageArray();
            let boardObj = document.getElementById("board");
            let boardRect = boardObj.getBoundingClientRect();
            let bagObj = document.getElementById("bag"); 
            let toRect = bagObj.getBoundingClientRect();
            let relEndX = toRect.left - boardRect.left;
            let relEndY = toRect.top - boardRect.top;

            for(i=0;i<fromArray.length;i++)
            {
                let fromObj = document.getElementById(fromArray[i]);
                let fromRect = fromObj.getBoundingClientRect();
                imageArray[i].style.left=fromRect.left - boardRect.left +"px";
                imageArray[i].style.top=fromRect.top - boardRect.top +"px";
                imageArray[i].style.position= 'absolute'; 
                imageArray[i].dx=relEndX-parseInt(imageArray[i].style.left);
                imageArray[i].dy=relEndY-parseInt(imageArray[i].style.top);

                let backgroundImage = fromObj.style.backgroundImage;
                if (isSafari)
                {
                    rawImage = backgroundImage.substring(4, backgroundImage.length - 1);
                }
                else
                {
                    rawImage = backgroundImage.substring(5, backgroundImage.length - 2);
                }
                imageArray[i].src=rawImage;

            }
            manymover.initMove();
            manymover.start(imageArray,fromArray.length);
        }

        var manymover=new moveMany();
        var imageArray=new Array();
