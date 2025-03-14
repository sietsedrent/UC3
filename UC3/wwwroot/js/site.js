// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function HidePassword() {
    var x = document.getElementById("hiddenpw");
    if (x.type == null || x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

