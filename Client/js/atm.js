$(document).ready(function() 
{
    resourceCall("ExibirCEF", "atm");
});

function exibirATM(dados) 
{
    $("#nome").text(dados.split("|")[0]);
    $("#saldo").text(dados.split("|")[1]);
}

function sacarATM() 
{
    if ($("#msg").text() == "Informe o valor do saque")
    {
        var valor = 0;
        if($("#valor").val())
            valor = $("#valor").val();

        resourceCall("AcaoCEFATM", "SACAR", valor);
        return;
    }

    $("#msg").text("Informe o valor do saque");
    $("#divvalor").removeAttr('style');
    $("#depositar").css("display","none");
}

function depositarATM()
{
    if ($("#msg").text() == "Informe o valor do depósito")
    {
        var valor = 0;
        if($("#valor").val())
            valor = $("#valor").val();

        resourceCall("AcaoCEFATM", "DEPOSITAR", valor);
        return;
    }

    $("#msg").text("Informe o valor do depósito");
    $("#divvalor").removeAttr('style');
    $("#sacar").css("display","none");
}