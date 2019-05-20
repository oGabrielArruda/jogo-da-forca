// Gabriel Alves de Arruda 19170    
// Angelo Gomes Pescarini 19161

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class Jogador
    {
       private int pontosJogador;
       private string nomeJogador;

       public Jogador(string nome, int pontuacao) // construtor da classe jogador que recebu como parâmetro o nome e a pontuação do jogador
       {
         nomeJogador = nome; // atribuimos o nome ao nomeJogador
         pontosJogador = pontuacao; // atribuímos a pontação ao pontosJogador
       }

       public int PontosJogador { get => pontosJogador; set => pontosJogador = value; }
       public string NomeJogador { get => nomeJogador; set => nomeJogador = value; }
}

