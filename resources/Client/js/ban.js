$(document).ready(function() 
{
    resourceCall("ExibirCEF", "ban");
});

function exibirBan(dados) 
{
   $("#idban").text(dados.split("|")[0]);
   $("#databan").text(dados.split("|")[1]);
   $("#expiracao").text(dados.split("|")[2]);
   $("#usuario").text(dados.split("|")[3]);
   $("#socialclub").text(dados.split("|")[4]);
   $("#motivo").text(dados.split("|")[5]);
   $("#staffer").text(dados.split("|")[6]);
};