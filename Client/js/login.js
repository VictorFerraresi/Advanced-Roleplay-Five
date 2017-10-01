function logPlayer() {
    var usuario = " ";
    var senha = " ";

    if($.trim($('#usuario').val()) != '')
        usuario = $("#usuario").val();

    if($.trim($('#senha').val()) != '')
        senha = $("#senha").val();

     resourceCall("logarPlayer", usuario, senha); 
};