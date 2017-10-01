$(document).ready(function() 
{
    $('#tabelabody tr').hover(function() {
        $(this).addClass('five-active');
    }, function() {
        $(this).removeClass('five-active');
    });

    resourceCall("ExibirCEF", "players");
});

function exibirTabPlayers(itemsjson, jogadores) 
{
    var list = JSON.parse(itemsjson);
    var html = "";
    $.each(list, function(i, item) 
    {
        html = html + '<tr><td>' + item.id + '</td><td>' + item.nome + '</td><td>' + item.level + '</td><td>' + item.ping + '</td></tr>';
    });
    $("#tabelabody").append(html);
    $("#jogadores").text(jogadores);
}