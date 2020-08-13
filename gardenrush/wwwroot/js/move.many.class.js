
class moveMany
{

   constructor()
   {
      let animate ;  // timer variable
      let time;      // 5s total move time
      let rate;      // 20ms for each iteration
      let steps;     // number of steps to destination
      let stepCount; // current step count
      let images;    // array of html image objects
   }

    initMove () {
      moveMany.constructor.time = 1000;     // 1000ms total move time
      moveMany.constructor.rate = 20;       // 20ms for each iteration
      moveMany.constructor.steps = moveMany.constructor.time / moveMany.constructor.rate; // number of steps to destination
    }

   start (sourceArray,total)
   {
        moveMany.constructor.images=new Array();
      for(i=0;i<total;i++)
      {
          sourceArray[i].stepX=sourceArray[i].dx/moveMany.constructor.steps;
          sourceArray[i].stepY=sourceArray[i].dy/moveMany.constructor.steps;
          sourceArray[i].ax=parseInt(sourceArray[i].style.left);
          sourceArray[i].ay=parseInt(sourceArray[i].style.top);
          sourceArray[i].style.display="block";
          moveMany.constructor.images.push(sourceArray[i]);
      }
       moveMany.constructor.stepCount = 0;
       if (moveMany.constructor.animate == undefined) {
           moveMany.constructor.animate = setInterval(this.step, moveMany.constructor.rate);    // call step in rate msec
       }
   }
   step ()
   {
      moveMany.constructor.stepCount+=1;
      if(moveMany.constructor.stepCount>=moveMany.constructor.steps)
      {
          clearInterval(moveMany.constructor.animate);
          moveMany.constructor.animate = undefined;
          for (i = 0; i < moveMany.constructor.images.length; i++)
          {
              moveMany.constructor.images[i].style.display = "none";
          }

      }

      for(i=0;i<moveMany.constructor.images.length;i++)
      {
        moveMany.constructor.images[i].ax+=moveMany.constructor.images[i].stepX;
        moveMany.constructor.images[i].ay+=moveMany.constructor.images[i].stepY;
        moveMany.constructor.images[i].style.left=parseInt(moveMany.constructor.images[i].ax)+'px';
        moveMany.constructor.images[i].style.top=parseInt(moveMany.constructor.images[i].ay)+'px';
      }

   }
}

