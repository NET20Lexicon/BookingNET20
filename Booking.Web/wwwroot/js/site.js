// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.



// Write your JavaScript code.

document.querySelector('#ajax').addEventListener('click', callBackEnd);
let p = document.querySelector('#createajax');

async function callBackEnd() {
    fetch('https://localhost:5001/GymClasses/fetch',
        {
            method: 'GET',
        })
        .then(res => res.text())
        .then(data => {
            p.innerHTML = data;
        })
        .catch(err => console.log(err));
};




$(document).ready(function () {
    $('#checkbox').click(function () {
        $('form').submit();
    })
});

function clearForm() {
    $('.clear').val('');
}

function removeForm() {
    $('#createajax').remove();
}
