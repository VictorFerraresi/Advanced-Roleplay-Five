$(document).ready(function() 
{
    $( "#portamalas" ).click(function() {
        resourceCall("AcaoVeiculoConce", "portamalas", 0, 0, 0);
    });

     $( "#capo" ).click(function() {
        resourceCall("AcaoVeiculoConce", "capo", 0, 0, 0);
    });

    $( "#portas" ).click(function() {
        resourceCall("AcaoVeiculoConce", "portas", 0, 0, 0);
    });

    $( "#buzina" ).click(function() {
        resourceCall("AcaoVeiculoConce", "buzina", 0, 0, 0);
    });

    $( "#testdrive" ).click(function() {
        resourceCall("AcaoVeiculoConce", "testdrive", 0, 0, 0);
    });

    $( "#comprar" ).click(function() {
        resourceCall("AcaoVeiculoConce", "comprar", 0, 0, 0);
    });

    $( "#cancelar" ).click(function() {
        resourceCall("AcaoVeiculoConce", "cancelar", 0, 0, 0);
    });
});

function updateCor1(picker) 
{
    $("#r1").val(Math.round(picker.rgb[0]));
    $("#g1").val(Math.round(picker.rgb[1]));
    $("#b1").val(Math.round(picker.rgb[2]));
    resourceCall("AcaoVeiculoConce", "cor1", Math.round(picker.rgb[0]), Math.round(picker.rgb[1]), Math.round(picker.rgb[2]));
}

function updateCor2(picker) 
{
    $("#r2").val(Math.round(picker.rgb[0]));
    $("#g2").val(Math.round(picker.rgb[1]));
    $("#b2").val(Math.round(picker.rgb[2]));
    resourceCall("AcaoVeiculoConce", "cor2", Math.round(picker.rgb[0]), Math.round(picker.rgb[1]), Math.round(picker.rgb[2]));
}