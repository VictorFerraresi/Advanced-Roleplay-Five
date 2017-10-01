$(document).ready(function() 
{
    resourceCall("ExibirCEF", "conce");
});

var list = null;
function exibirConce(dados) 
{
    list = JSON.parse(dados);
    var html = "";
    $.each(list, function(i, item) 
    {
        html = html + '<div class="col-md-4" id="'+ item.veiculo +'"><img src="img/veiculos/'+ item.veiculo +'.png" onclick="exibirVeiculo(' + i + ')"><p class="text-center">' + item.veiculo +'</p></div>';
    });
    $("#veiculos").append(html);
}

function pesquisarVeiculos() 
{
    var filtro = $("#filtro").val().toLowerCase();

    $.each(list, function(i, item) 
    {
        if (item.veiculo.toLowerCase().indexOf(filtro) > -1)
        {
             $("#" + item.veiculo).css("display", "");
        }
        else
        {
            $("#" + item.veiculo).css("display", "none");
        }
    });
}

function exibirVeiculo(id)
{
    if (list[id] == null || list[id] == undefined) return;

    $("#veiculo").attr("src", "img/veiculos/" + list[id].veiculo + ".png");
    $("#nomeveiculo").html(list[id].veiculo);
    $("#preco").html("$" + list[id].preco);
    $("#lugares").html(list[id].lugares);

    $("#velocidade").css('width', list[id].velocidade+'%');
    $("#frenagem").css('width', list[id].frenagem+'%');
    $("#aceleracao").css('width', list[id].aceleracao+'%');
    $("#tracao").css('width', list[id].tracao+'%');
    $("#geral").css('width', list[id].geral+'%');
}

function visualizarVeiculo()
{
    resourceCall("VerVeiculoConce", $("#nomeveiculo").html().toString());
}

function cancelarVeiculo() 
{
    resourceCall("FecharCEF", "conce");
}