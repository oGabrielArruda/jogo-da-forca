// Gabriel Alves de Arruda 19170    
// Angelo Gomes Pescarini 19161

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _19161_19170__ProjetoForca
{
    class PalavraDica : IComparable<PalavraDica>, IRegistro
    {
        private string palavraUsada;
        private string dicaUsada;


        const int tamanhoPalavra = 15;
        const int tamanhoDica = 100;

        const int inicioPalavra = 0;
        const int inicioDica = inicioPalavra + tamanhoPalavra;

        public PalavraDica(string palavra, string dica) // são lidos e divididos em strings a palavra e sua respectiva dica
        {
            palavraUsada = palavra;
            dicaUsada = dica;
        }

        public string PalavraUsada { get => palavraUsada; set => palavraUsada = value; } // acessam as palavras utilizadas
        public string DicaUsada { get => dicaUsada; set => dicaUsada = value; }  //sem os gets e os sets, as strings
                                                                                 //poderiam ser modificadas fora da classe
        public void LerRegistro(StreamReader arq)
        {
            if (!arq.EndOfStream)
            {
                String linha = arq.ReadLine();
                palavraUsada = linha.Substring(inicioPalavra, tamanhoPalavra);
                dicaUsada = linha.Substring(inicioDica, tamanhoDica);
            }
        }

        public PalavraDica()
        {

        }
        public int CompareTo(PalavraDica outra)
        {
            return palavraUsada.CompareTo(outra.PalavraUsada);
        }

        public String ParaArquivo()
        {
            return palavraUsada + "" + dicaUsada;
        }
    }
}
