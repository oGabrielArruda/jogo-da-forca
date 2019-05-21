// Gabriel Alves de Arruda 19170    
// Angelo Gomes Pescarini 19161

using System;
using static System.Console;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace _19161_19170__ProjetoForca
{
    class VetorPalavra
    {
        int tamanhoMaximo;  // tamanho físico do vetor dados
        int qtsDesafios;      // tamanho lógico do vetor dados
        private PalavraDica[] desafio;
         string[] vetorCaracteres = new string[15];

        public VetorPalavra(int tamanhoDesejado)
        {
            desafio = new PalavraDica[tamanhoDesejado];
            qtsDesafios = 0;
            tamanhoMaximo = tamanhoDesejado;
        }

        public void LerDados(string nomeArq)   // ler de um arquivo texto
        {
            var arq = new StreamReader(nomeArq);

            while (!arq.EndOfStream)
            {
                string linha = arq.ReadLine(); // lê-se uma linha inteira do arquivo

                PalavraDica PalavraEDica = new PalavraDica(linha); // o objeto PalavraEDica da classe PalavraDica, passa a linha lida como parâmetro
                                                                   // e dessa linha, na classe, serão guardados a palavra e a dica
                InserirAposFim(PalavraEDica); // insere-se o objeto no vetor      
            }
            arq.Close();
        }
        public void InserirAposFim(PalavraDica valorAInserir) // o valor a ser inserido, será um objeto da classe PalavraDica
        {
            if (qtsDesafios >= tamanhoMaximo)
                ExpandirVetor();

            desafio[qtsDesafios] = valorAInserir;
            qtsDesafios++;
        }
        private void ExpandirVetor()
        {
            tamanhoMaximo += 10;
            PalavraDica[] vetorMaior = new PalavraDica[tamanhoMaximo];
            for (int indice = 0; indice < qtsDesafios; indice++)
                vetorMaior[indice] = desafio[indice];

            desafio = vetorMaior;
        }

        public void Excluir(int posicaoAExcluir)
        {
            qtsDesafios--;
            for (int indice = posicaoAExcluir; indice < qtsDesafios; indice++)
                desafio[indice] = desafio[indice + 1];

            // pensar em como diminuir o tamanho físico do vetor, para economizar

        }

        public void Listar(ListBox lista)
        {
            lista.Items.Clear();
            for (int indice = 0; indice < qtsDesafios; indice++)
                lista.Items.Add(desafio[indice]);
        }
        public void Listar(ComboBox lista)
        {
            lista.Items.Clear();
            for (int indice = 0; indice < qtsDesafios; indice++)
                lista.Items.Add(desafio[indice]);
        }
        public void Listar(TextBox lista)
        {
            lista.Multiline = true;
            lista.ScrollBars = ScrollBars.Both;
            lista.Clear();
            for (int indice = 0; indice < qtsDesafios; indice++)
                lista.AppendText(desafio[indice] + Environment.NewLine);
        }

        public void GravarDados(string nomeArquivo)
        {
            var arquivo = new StreamWriter(nomeArquivo);        // abre arquivo para escrita
            for (int indice = 0; indice < qtsDesafios; indice++)  // percorre elementos do vetor
                arquivo.WriteLine($"{desafio[indice],5}");       // grava cada elemento
            arquivo.Close();
        }
        public override string ToString()  // retorna lista de valores separados por 
        {                                  // espaço
            return ToString(" ");
        }

        public string ToString(string separador) // retorna lista de valores separados 
        {                                        // por separador
            string resultado = "";
            for (int indice = 0; indice < qtsDesafios; indice++)
                resultado += desafio[indice] + separador;
            return resultado;
        }

        public void AcessarPalavraEDica(int nmrLinha, ref string palavraAcessada, ref string dicaAcessada)
        {
            PalavraDica acessado = desafio[nmrLinha]; // acessa o objeto que está no vetor do número que foi sorteado
            palavraAcessada = acessado.PalavraUsada; // devolve para o programa a palavra e a dica do objeto sorteado
            dicaAcessada = acessado.DicaUsada;
        }

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
    }
}
