$(document).ready(function() 
{
    resourceCall("ExibirCEF", "stats");
});

function exibirStatsPlayer(json) 
{
    $("#nome").text(json.split("|")[0]);
    $("#vida").css('width', json.split("|")[1]+'%');
    $("#colete").css('width', json.split("|")[2]+'%');
    $("#dinheiro").text("$" + json.split("|")[3]);
    $("#banco").text("$" + json.split("|")[4]);
}