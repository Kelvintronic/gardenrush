
class moveObject
{

   constructor()
   {
       let imgObj;
      let animate ;
      let startX;
      let startY;
      let dx;
      let dy;
      let time;      // 5s total move time
      let rate;      // 20ms for each iteration
      let steps;     // number of steps to destination
      let stepCount;
      let stepX;
      let stepY;
      let outXObj;
      let outYObj; 
      let x;
      let y;   
      let placesound;
       let pickupsound;
   }

    initMove = function () {
      moveObject.constructor.imgObj = document.getElementById('myImage');
      moveObject.constructor.imgObj.style.position= 'absolute'; 
      moveObject.constructor.imgObj.style.left = '0px'; 
      moveObject.constructor.imgObj.style.top = '0px';
      moveObject.constructor.time = 1000;     // 1000ms total move time
      moveObject.constructor.rate = 20;       // 20ms for each iteration
      moveObject.constructor.steps = moveObject.constructor.time / moveObject.constructor.rate; // number of steps to destination
    }

    setImage = function(image) {
        moveObject.constructor.imgObj.src = image;
    }

   start = function(fromX, fromY, toX, toY)
   {
       try {
           pickupsound.playsound();
       }
       catch(ex) { }
      moveObject.constructor.startX=fromX;
      moveObject.constructor.startY=fromY;
      moveObject.constructor.x=fromX;
      moveObject.constructor.y=fromY;
      moveObject.constructor.dx=toX-fromX;
      moveObject.constructor.dy=toY-fromY;
      moveObject.constructor.stepX=moveObject.constructor.dx/moveObject.constructor.steps;
      moveObject.constructor.stepY=moveObject.constructor.dy/moveObject.constructor.steps;
      moveObject.constructor.stepCount = 0;
      moveObject.constructor.imgObj.style.display = "block";

      moveObject.constructor.animate = setInterval(this.step,moveObject.constructor.rate);    // call step in rate msec
   }
   step = function()
   {
      moveObject.constructor.stepCount+=1;
      if(moveObject.constructor.stepCount>=moveObject.constructor.steps)
      {
          try {
              placesound.playsound();
          }
          catch (ex) {}
          clearTimeout(moveObject.constructor.animate);
          moveObject.constructor.imgObj.style.display = "none";
      }
      if((moveObject.constructor.stepX<0&&moveObject.constructor.x>moveObject.constructor.startX+moveObject.constructor.dx)||(moveObject.constructor.stepX>0&&moveObject.constructor.x<moveObject.constructor.startX+moveObject.constructor.dx))
         moveObject.constructor.x+=moveObject.constructor.stepX;
      moveObject.constructor.imgObj.style.left = parseInt(moveObject.constructor.x) + 'px';
      if((moveObject.constructor.stepY<0&&moveObject.constructor.y>moveObject.constructor.startY+moveObject.constructor.dy)||(moveObject.constructor.stepY>0&&moveObject.constructor.y<moveObject.constructor.startY+moveObject.constructor.dy))
         moveObject.constructor.y+=moveObject.constructor.stepY;
      moveObject.constructor.imgObj.style.top = parseInt(moveObject.constructor.y) + 'px';
   }
}

