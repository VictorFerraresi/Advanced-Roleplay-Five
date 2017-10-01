$(document).ready(function() 
{
    $('#tabelabody tr').hover(function() {
        $(this).addClass('five-active');
    }, function() {
        $(this).removeClass('five-active');
    });

    resourceCall("ExibirCEF", "sos");
});

function exibirTabSOS(itemsjson, qtd) 
{
    var list = JSON.parse(itemsjson);
    var html = "";
    $.each(list, function(i, item) 
    {
        html = html + '<tr><td>' + item.IDPlayer + '</td><td>' + item.NomePersonagem + '</td><td>' + item.Descricao + '</td></tr>';
    });
    $("#tabelabody").append(html);
    $("#qtd").text(qtd);
}