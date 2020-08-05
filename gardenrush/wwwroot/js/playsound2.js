
class soundplayer
{
    constructor(src)
    {
        this.sound = document.createElement("audio");
        this.sound.src = src;
        this.sound.setAttribute("preload", "auto");
        this.sound.setAttribute("controls", "none");
        this.sound.style.display = "none";
        document.body.appendChild(this.sound);
    }

    playsound()
    {
        this.sound.play();
    }

    pausesound()
    {
        this.sound.pause();
    }

}
