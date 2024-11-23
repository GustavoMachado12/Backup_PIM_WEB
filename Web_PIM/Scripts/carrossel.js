const box = document.querySelector(".slides");
const imagens = document.querySelectorAll(".slides img");

let contador = 0;

function slider() {
    contador++;

    if (contador > imagens.length - 1) {
        contador = 0;
    }

    box.style.transform = `translateX(${-contador * 800}px)`

}

setInterval( slider , 4000 )

