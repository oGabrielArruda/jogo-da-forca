using System;
using System.IO;

interface IRegistro
{
  void LerRegistro(StreamReader arq);
  string ParaArquivo();
}
