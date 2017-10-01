$(document).ready(function() 
{
    resourceCall("ExibirCEF", "help");
});

function exibirHelp(itemsjson) 
{
    var comandos = JSON.parse(itemsjson);
    var html = "";
    $.each(comandos, function(i, item) 
    {
        html = html + '<tr><td>' + item.Categoria + '</td><td>' + item.Nome + '</td><td>' + item.Descricao + '</td></tr>';
    });
    $("#tabelabody").append(html);
}

function pesquisarComandos() 
{
  var input, filter, table, tr, td, td2, td3, i;
  input = document.getElementById("filtro");
  filter = input.value.toUpperCase();
  table = document.getElementById("tabela");
  tr = table.getElementsByTagName("tr");

  for (i = 0; i < tr.length; i++) 
  {
    td = tr[i].getElementsByTagName("td")[0];
    td2 = tr[i].getElementsByTagName("td")[1];
    td3 = tr[i].getElementsByTagName("td")[2];
    if (td) 
    {
      if (td.innerHTML.toUpperCase().indexOf(filter) > -1 || td2.innerHTML.toUpperCase().indexOf(filter) > -1 || td3.innerHTML.toUpperCase().indexOf(filter) > -1) 
      {
        tr[i].style.display = "";
      } 
      else 
      {
        tr[i].style.display = "none";
      }
    } 
  }
}