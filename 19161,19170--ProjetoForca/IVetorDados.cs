using System;
using System.Windows.Forms;

interface IVetorDados<Registro> 
        where Registro : IComparable<Registro>, IRegistro, new()
{
  void PosicionarNoPrimeiro();
  void PosicionarNoUltimo();
  void AvancarPosicao();
  void RetrocederPosicao();
  void ExibirDados();
  void ExibirDados(ListBox lista, string cabecalho);
  void ExibirDados(ComboBox lista);
  void ExibirDados(DataGridView grade);
  void ExibirDados(TextBox lista, string cabecalho);
  bool Existe(Registro procurado, ref int ondeEsta);
  void Excluir(int posicao);
  void Incluir(Registro novoValor);
  void Incluir(Registro novoValor, int posicaoDeInclusao);
  void Ordenar();
  void LerDados(string nomeArquivo);
  void GravarDados(string nomeArquivo);
  Registro this[int indice] { get; set; }
  Situacao SituacaoAtual { get; set; }
  int PosicaoAtual { get; set; }
  bool EstaNoInicio { get; }
  bool EstaNoFim { get; }
  bool EstaVazio { get; }
  int Tamanho { get; }
}
