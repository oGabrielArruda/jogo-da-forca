// Gabriel Alves de Arruda 19170    
// Angelo Gomes Pescarini 19161

using System;
using static System.Console;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

public enum Situacao // para atribuir as funcionalidades de acordo com a situação
{
    navegando, pesquisando, incluindo, editando, excluindo
}
class VetorDados<Registro>: IVetorDados<Registro> where
        Registro : IComparable<Registro>, IRegistro, new() // a classe vetor dados com base no Registro deve seguir as
                                                          // interfaces IVetorDados, IRegistro, e deve ter um método de comparação
    {
        int tamanhoMaximo;  // tamanho físico do vetor dados
        int qtsDados;      // tamanho lógico do vetor dados
        Situacao situacaoAtual; 
        int posicaoAtual;
        private Registro[] dados; // vetor dados do objeto genérico registro
         string[] vetorCaracteres = new string[15];

        public VetorDados(int tamanhoDesejado)
        {
            dados = new Registro[tamanhoDesejado]; // instanciamos o vetor
            qtsDados = 0;
            tamanhoMaximo = tamanhoDesejado;
        }

        public void LerDados(string nomeArq)   // ler de um arquivo texto
        {
           if (!File.Exists(nomeArq))   // se o arquivo não existe
           {
               var arqNovo = File.CreateText(nomeArq);  // criamos o arquivo vazio
               arqNovo.Close();
           }
          var arq = new StreamReader(nomeArq);
          qtsDados = 0;
            while (!arq.EndOfStream)
            {
                 var umRegistro = new Registro(); // o objeto PalavraEDica da classe PalavraDica, passa a linha lida como parâmetro
                 umRegistro.LerRegistro(arq);
                 Incluir(umRegistro);                                                  // e dessa linha, na classe, serão guardados a palavra e a dica
            }
            arq.Close();
        }
        public void InserirAposFim(Registro valorAInserir) // o valor a ser inserido, será um objeto da classe PalavraDica
        {
            if (qtsDados >= tamanhoMaximo)
                ExpandirVetor();

            dados[qtsDados] = valorAInserir;
            qtsDados++;
        }
        private void ExpandirVetor()
        {
            tamanhoMaximo += 10;
            Registro[] vetorMaior = new Registro[tamanhoMaximo];
            for (int indice = 0; indice < qtsDados; indice++)
                vetorMaior[indice] = dados[indice];

            dados = vetorMaior;
        }

        public void Excluir(int posicaoAExcluir)
        {
            qtsDados--;
            for (int indice = posicaoAExcluir; indice < qtsDados; indice++)
                dados[indice] = dados[indice + 1];

            // pensar em como diminuir o tamanho físico do vetor, para economizar

        }

        public void Listar(ListBox lista)
        {
            lista.Items.Clear();
            for (int indice = 0; indice < qtsDados; indice++)
                lista.Items.Add(dados[indice]);
        }
        public void Listar(ComboBox lista)
        {
            lista.Items.Clear();
            for (int indice = 0; indice < qtsDados; indice++)
                lista.Items.Add(dados[indice]);
        }
        public void Listar(TextBox lista)
        {
            lista.Multiline = true;
            lista.ScrollBars = ScrollBars.Both;
            lista.Clear();
            for (int indice = 0; indice < qtsDados; indice++)
                lista.AppendText(dados[indice] + Environment.NewLine);
        }

        public void GravarDados(string nomeArquivo)
        {
            var arquivo = new StreamWriter(nomeArquivo);        // abre arquivo para escrita
            for (int indice = 0; indice < qtsDados; indice++)  // percorre elementos do vetor
                arquivo.WriteLine(dados[indice].ParaArquivo());       // grava cada elemento
            arquivo.Close();
        }
        public override string ToString()  // retorna lista de valores separados por 
        {                                  // espaço
            return ToString(" ");
        }

        public string ToString(string separador) // retorna lista de valores separados 
        {                                        // por separador
            string resultado = "";
            for (int indice = 0; indice < qtsDados; indice++)
                resultado += dados[indice] + separador;
            return resultado;
        }

       /* public void AcessarPalavraEDica(int nmrLinha, ref string palavraAcessada, ref string dicaAcessada)
        {      
            PalavraDica acessado = dados[nmrLinha]; // acessa o objeto que está no vetor do número que foi sorteado
            palavraAcessada = acessado.PalavraUsada; // devolve para o programa a palavra e a dica do objeto sorteado
            dicaAcessada = acessado.DicaUsada; 
        } */

        int qtosCaracteres = 0;

        

        public void SepararDigito(string palavra, DataGridView qualDgv) //função que separará a palavra em jogo por letras
        {
            qtosCaracteres = 0;
            while(qtosCaracteres < palavra.Trim().Length) // enquanto o indice for menor que o numero de letras da palavra(sem espaços) ->
            {
                vetorCaracteres[qtosCaracteres] = palavra.Substring(qtosCaracteres, 1); // atribui-se ao vetor de strings o valor da letra
                qtosCaracteres++;
            }
            qualDgv.ColumnCount = qtosCaracteres; // divide-se o DataGridView pelo tamanho de letras
            
        }

        public int QtosCaracteres { get => qtosCaracteres; set => qtosCaracteres = value; }  // variável que guardaremos o tamanho da palavra

        
        public int[] PosicoesNaPalavra(string letra, ref int qtsOcorrencias) // método que retornará um vetor de valores com a posição de cada letra
        {                                                
            int[] posicoes = new int[15]; // criação do vetor
            int indice = 0; // indice do vetor

            for (int i = 0; i < qtosCaracteres; i++) // percorremos os caracteres da palavra
                if (vetorCaracteres[i] == letra) // se o caracter da vez for igual a letra
                {
                    posicoes[indice] = i;
                    indice++;
                }

            qtsOcorrencias = indice; //quantas vezes a  letra apareceu
            return posicoes; // o vetor 'posicoes' retornado, terá o valor dos carácteres que ocorreram a letra.
        }
    public int Tamanho  // permite à aplicação consultar o número de registros armazenados
    {
        get => qtsDados;
    }
    public int PosicaoAtual // retorna quakl indíce está sendo exibido na tela de manutenção
    {
        get => posicaoAtual;
        set
        {
            if (value >= 0 && value < qtsDados)
                posicaoAtual = value;
        }
    }
    public Registro this[int indice] // retorna o valor do vetor no registro passado como parâmetro
    {
        get
        {
            if (indice < 0 || indice >= qtsDados)  // inválido
                throw new Exception("Índice inválido!");

            return dados[indice];
        }
        set
        {
            if (indice < 0 || indice >= qtsDados)
                throw new Exception("Índice fora dos limites do vetor!");

            dados[indice] = value;
        }
    }
    public void Ordenar()  // Straight-select sort
    {
        for (int lento = 0; lento < qtsDados; lento++)
        {
            int indiceDoMenor = lento;
            for (int rapido = lento + 1; rapido < qtsDados; rapido++)
                if (dados[rapido].CompareTo(dados[indiceDoMenor]) < 0)
                    indiceDoMenor = rapido;
            if (indiceDoMenor != lento)
            {
                Registro aux = dados[lento];
                dados[lento] = dados[indiceDoMenor];
                dados[indiceDoMenor] = aux;
            }
        }
    }

    public void Incluir(Registro valorAInserir) // insere após o final do vetor e o expande se necessário
    {
        if (qtsDados >= dados.Length)
            ExpandirVetor();

        dados[qtsDados] = valorAInserir;
        qtsDados++;
    }

    // insere o novo dado na posição indicada por ondeIncluir
    public void Incluir(Registro valorAInserir, int ondeIncluir) // inclui o objeto passado como parâmetro na posição desejada
    {
        if (qtsDados >= dados.Length)
            ExpandirVetor();

        // desloca para frente os dados posteriores ao novo dado
        for (int indice = qtsDados - 1; indice >= ondeIncluir; indice--)
            dados[indice + 1] = dados[indice];

        dados[ondeIncluir] = valorAInserir;
        qtsDados++;
    }

    public bool Existe(Registro procurado, ref int onde) // Vê se existe o objeto passado como parâmetro e devolve o índice em que ele foi encontrado
    {
        bool achou = false;
        int inicio = 0;
        int fim = qtsDados - 1;
        while (!achou && inicio <= fim)
        {
            onde = (inicio + fim) / 2;
            if (dados[onde].CompareTo(procurado) == 0)
                achou = true;
            else
              if (procurado.CompareTo(dados[onde]) < 0)
                fim = onde - 1;
            else
                inicio = onde + 1;
        }
        if (!achou)
            onde = inicio; // onde deverá ser incluído o novo registro caso não tenha sido achado
        return achou;
    }

    public void ExibirDados(ListBox lista, string cabecalho)
    {
        lista.Items.Clear();
        lista.Items.Add(cabecalho);
        for (int indice = 0; indice < qtsDados; indice++)
            lista.Items.Add(dados[indice]);
    }

    public void ExibirDados()
    {
        for (int indice = 0; indice < qtsDados; indice++)
            WriteLine($"{dados[indice],5} ");
    }

    public void ExibirDados(ComboBox lista)
    {
        lista.Items.Clear();
        for (int indice = 0; indice < qtsDados; indice++)
            lista.Items.Add(dados[indice]);
    }
    public void ExibirDados(TextBox lista, string cabecalho)
    {
        lista.Multiline = true;
        lista.ScrollBars = ScrollBars.Both;
        lista.Clear();
        lista.AppendText(cabecalho + Environment.NewLine);
        for (int indice = 0; indice < qtsDados; indice++)
            lista.AppendText(dados[indice] + Environment.NewLine);
    }

    public void ExibirDados(DataGridView lista)
    {
    }
    public Situacao SituacaoAtual // atributo público para a situação atual
        {
            get => situacaoAtual;
            set => situacaoAtual = value;
        }
        public bool EstaVazio // permite à aplicação saber se o vetor dados está vazio
        {
            get => qtsDados <= 0; // se qtosDados <= 0, retorna true
        }

        public void PosicionarNoPrimeiro() // determjna a poisção atual no primeiro
        {
            if (!EstaVazio)
                posicaoAtual = 0; // primeiro elemento do vetor
            else
                posicaoAtual = -1; // antes do início do vetor
        }
        public void PosicionarNoUltimo() // determjna a poisção atual no último
    {
            if (!EstaVazio)
                posicaoAtual = qtsDados - 1; // última posição usada do vetor
            else
                posicaoAtual = -1; // indica antes do vetor vazio
        }
        public void AvancarPosicao() // avança 1 na posição atual
        {
            if (!EstaNoFim)
                posicaoAtual++;
        }
        public void RetrocederPosicao() // subtrai 1 da posição atual
        {
            if (!EstaNoInicio)
                posicaoAtual--;
        }
        public bool EstaNoInicio // retorna true se a posição atual estiver no primeiro índice
        {
            get => posicaoAtual <= 0; // primeiro índice
        }
        public bool EstaNoFim // retorna true se a posição atual estiver no último índice
    {
            get => posicaoAtual >= qtsDados - 1; // último índice
        }
    }

