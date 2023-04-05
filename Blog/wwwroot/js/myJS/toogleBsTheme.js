
var globalThemeAttr = localStorage.getItem("theme");
document.documentElement.setAttribute(
    "data-bs-theme",
    globalThemeAttr ? globalThemeAttr : "light"
);

toogleThemeForComments();

function toogleTheme() {

    if (!globalThemeAttr) {
        globalThemeAttr = "light";
        localStorage.setItem("theme", "light");
        document.documentElement.setAttribute("data-bs-theme", "light");

        return;
    }

    if (globalThemeAttr == "light") {
        document.documentElement.setAttribute("data-bs-theme", "dark")
        globalThemeAttr = "dark";
        localStorage.setItem("theme", "dark");
        // console.log(globalThemeAttr);
    }
    else {
        document.documentElement.setAttribute("data-bs-theme", "light");
        globalThemeAttr = "light";
        localStorage.setItem("theme", "light");
        // console.log(globalThemeAttr);
    }

    let toogleThemeButton = document.getElementById("toogleThemeButton");
    toogleThemeButton.textContent = globalThemeAttr == "light" ?
        "White" :
        "Dark";

    toogleThemeForComments();
}

function toogleThemeForComments() {

    let comments = document.getElementsByClassName("removedTransition");

    if (comments.length != 0) {
        for (let i = 0; i < comments.length; i++) {
            if (globalThemeAttr == "dark") {
                if (comments[i].style.backgroundColor == "lightgray") {
                    comments[i].style.backgroundColor = "rgb(10, 10, 10)"; //"dimgrey";
                }
                else if (comments[i].style.backgroundColor == "white") {
                    comments[i].style.backgroundColor = "rgb(50, 50, 50)"; //"grey";
                }
            }
            else {
                if (comments[i].style.backgroundColor == "rgb(10, 10, 10)") {
                    comments[i].style.backgroundColor = "lightgray";
                }
                else if (comments[i].style.backgroundColor == "rgb(50, 50, 50)") {
                    comments[i].style.backgroundColor = "white";
                }
            }
        }
    }
}