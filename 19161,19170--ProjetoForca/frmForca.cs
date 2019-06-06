// Gabriel Alves de Arruda 19170    
// Angelo Gomes Pescarini 19161
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _19161_19170__ProjetoForca
{
    public partial class Forca : Form
    {
        VetorDados<PalavraDica> vetor; 

        public Forca()
        {
            InitializeComponent();
            vetor = new VetorDados<PalavraDica>(100); // instanciamos o vetor
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int indice = 0;
            barraDeFerramentas.ImageList = imlBotoes;
            foreach (ToolStripItem item in barraDeFerramentas.Items)
                if (item is ToolStripButton) // se não é separador:
                    (item as ToolStripButton).ImageIndex = indice++;


            tmrAgora.Start(); // incia o timer do horário do dia
            dlgAbrir.Title = "Escolha o arquivo texto para o jogo"; // título do OpenFileDialog
            if (dlgAbrir.ShowDialog() == DialogResult.OK) // se abriu o arquivo
            {
                vetor.LerDados(dlgAbrir.FileName); // lemos os dados na classe vetor passando como parâmetro o nome do arquivo aberto
            }
            vetor.Ordenar();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (txtSeuNome.Text != "")
            {
                string palavra = "", dica = ""; // strings em que serão armazenadas a palavra e a dica do desafio da vez
                AcessarDesafio(ref palavra, ref dica); // para silmplificar a leitura do código, cria-se um método para acessar o desafio
                IniciarJogo(palavra, dica);  // inicia o jogo.
            }
            else
                MessageBox.Show("Digite seu nome!", "Campos faltantes", MessageBoxButtons.OK);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        int qtsErros = 0, qtsPontos = 0;
        Button[] quaisBotoes = new Button[30]; // criamos um vetor de botões para saber quais botões foram pressionados
        int qtosBotoes = 0;

        private void btnClick(object sender, EventArgs e) // ao clicar qualquer botão do teclado exibido, esse evento será chamado
        {
            try
            {
                Button botao = (Button)sender; // o objeto botão será o botão pressionado
                quaisBotoes[qtosBotoes] = botao; // vetor de botões pressionados para podermos habilitar-los depois
                qtosBotoes++;

                botao.Enabled = false; // desabilitamos o botão
                string letraBotao = botao.Text.ToLower();  // a string letra botao, declarada anteriormente, será o texto do botão pressionado,
                                                           // ou seja, a letra que o botão representa
                                                           // como no arquivo texto usamos letras minúsculas, usamos o método ToLower() para transformar o valor do botão para letras minúsculas também  

                int qtsOcorrencias = 0;
                int[] posicoesNaPalavra = vetor.PosicoesNaPalavra(letraBotao, ref qtsOcorrencias); // vetor que terá as posições de ocorrência da letra

                if (qtsOcorrencias > 0) // ou seja, se a letra foi encontrada na palvra
                {
                    qtsPontos += qtsOcorrencias;  // somamos um ponto pra cada letra acertada
                    lbPontos.Text = "Pontos:" + qtsPontos.ToString(); // adicionamos os pontos
                    botao.BackColor = Color.Green; // deixamos o botão verde para informar que a letra estava na palavra
                    AdicionarNoDgv(dgvPalavra, posicoesNaPalavra, qtsOcorrencias, letraBotao); //exibimos no datagridview
                }
                else // se a letra não for encontrada na palavra, ou seja, se o jogador errou
                {
                    qtsErros++; // conta-se mais um erro
                    lbErros.Text = "Erros(8): " + qtsErros.ToString(); //marcamos os erros
                    botao.BackColor = Color.Red; // deixamos os botões vermelho para informar que não tinha na palavra
                    ExibirErrosNaForca(qtsErros); // exibimos a imagem com o número do erro correspondente
                }

                if (qtsPontos == vetor.QtosCaracteres) // se a pontuação for igual o tamanho da palavra
                {
                    tmrTempo.Stop(); // paramos o timer
                    tmrTempo.Enabled = false;
                    GameWin(); // o jogador ganha
                }

                if (qtsErros == 8) // se o jogador errar oito vezes ele perde
                {
                        tmrTempo.Stop(); // paramos o timer
                        tmrTempo.Enabled = false;
                        GameOver(); // o jogador perde
                }
            }
            catch{ //deixamos o catch vazio para evitar que os clicks abusivos do usuário causem erro no programa

            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //----------------------------------------------------------------------------------------------------------------//
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        void AcessarDesafio(ref string palavra, ref string dica)   //Método que acessará o desafio e devolverá a palavra e a dica da
        {                                                          // rodada
            Random sorteioNmr = new Random();
            int nmrLinha = sorteioNmr.Next(100);      // sorteia um número entre 0 e 99
            PalavraDica oSorteado = new PalavraDica();
            oSorteado = vetor[nmrLinha];
            palavra = oSorteado.PalavraUsada;
            dica = oSorteado.DicaUsada;
        }                                                               

        void IniciarJogo(string palavra, string dica) 
        {
            vetor.SepararDigito(palavra, dgvPalavra); // método que dividirá a palavra em jogo em vetores de strings,  
                                                      // e também dividirá o DataGridView

            btnIniciar.Visible = false;// deixamos o botão iniciar invisível para melhorar a interface
            chkComDica.Visible = false; //desabilitamos o check box da dica
            txtSeuNome.Enabled = false; // desabilitamos o text box do nome, para evitar que seja alterado ao longo do jogo
            panelTeclado.Enabled = true;

            if (ComDica()) // se o jogador deseja jogar com dica
            {
                HabilitarTimer(tmrTempo, lbTempo); // hablitamos o timer e a contagem é iniciada
                lbDica.Text = "Dica: " + dica; // exibimos a dica para o jogador

            }        
            // Nosso jogo está iniciado, os outros eventos estarão no Click dos botões.
        }

        void HabilitarTimer(Timer qualTimer, Label qualLabel) // habilitamos o timer
        {
            qualTimer.Enabled = true;
            qualTimer.Start();
            qualLabel.Visible = true;
        }

        bool ComDica() // se tem dica
        {
            return chkComDica.Checked;
        }


        void AdicionarNoDgv(DataGridView qualDgv, int[] posicoes, int qtsOcorrencias, string qualLetra)
        {
            for(int i = 0; i < qtsOcorrencias; i++) // o índice do vetor será até quantas vezes a letra escolhida ocorreu
            {
                dgvPalavra.Rows[0].Cells[posicoes[i]].Value = qualLetra.ToUpper(); // adicionamos na coluna de número do vetor das posições a letra escolhida
            }
        }

        void GameWin()
        {
            if (MessageBox.Show(txtSeuNome.Text + "\nVocê ganhou!\nPontuação: " + qtsPontos +
                   "\nDeseja jogar novamente?", "Parabéns", MessageBoxButtons.YesNo) == DialogResult.Yes) //vemos se o jogador quer jogar mais
            {
                ImagensGanhou(); // exibimos a imagem de vitória
                System.Threading.Thread.Sleep(3000); // esperamos 3 segundos para resetar
                SalvarJogador(txtSeuNome.Text.ToLower(), qtsPontos); // salvamos os dados do jogador em um arquivo texto
                Reset();    // resetamos o jogo
            }
            else
            {
                SalvarJogador(txtSeuNome.Text.ToLower(), qtsPontos); // salvamos os dados do jogador em um arquivo texto
                Close(); //fechamos o jogo
                arqRanking.Close(); //fechamos o arquivo texto com o nome dos jogadores
            }
        }

        void GameOver()
        {
            if (MessageBox.Show(txtSeuNome.Text + "\nVocê perdeu!\nDeseja jogar novamente?", "Derrota", MessageBoxButtons.YesNo)
                == DialogResult.Yes) // vemos se  o jogador quer jogar mais
            {
                ImagensPerdeu();
                System.Threading.Thread.Sleep(3000); // esperamos 3  segundos         
                Reset(); // resetamos o jogo
            }
            else
            {
                Close(); //se ele não quiser jogar mais fechamos o jogo
                arqRanking.Close(); //fechamos o arquivo texto com o nome dos jogadores
            }
        }

        void Reset()
        {
            ResetarImagens(); // escondemos novamente as imagens
            ResetarBotoes();  // rehabilitamos os botões e voltamos a sua cor anterior
            panelTeclado.Enabled = false; // desabilitamos o teclado
            txtSeuNome.Clear(); // limpamos a área do nome
            txtSeuNome.Enabled = true; // habilitamos a área do nome
            lbDica.Text = "Dica:______________________ "; 
            lbPontos.Text = "Pontos:____ "; // deixamos os labels como eles eram antes
            lbErros.Text = "Erros(8):____ ";
            lbTempo.Text = "Tempo Restante:___s";
            lbTempo.Visible = false; 
            LimparDgv(dgvPalavra); // limpamos o data grid view
            btnIniciar.Visible = true;  
            chkComDica.Checked = false;
            qtsPontos = 0; // resetamos a pontuação
            qtsErros = 0; //resetamos os erros
            chkComDica.Visible = true; // deixamos visível o check box da dica
            quantosSegundosFaltam = 60; //deixamos o timer com 60segundos novamente
        }


        Jogador[] vetorJogadores = new Jogador[30]; //vetor de objetos da classe jogador
        StreamWriter arqRanking = new StreamWriter("Ranking.txt"); // como não foi pedido lugar para o arquivo ser salvo, nosso ranking será salvo no DIRETÓRIO do programa
        bool cabecalho = true;
        int qtsJogadores = 0;
         
        void SalvarJogador(string nome, int pontos)
        {
            if(cabecalho)
            {
                arqRanking.WriteLine("Nome            Pontos"); // se for a primeira vez, escrevemos o cabeçalho indicando onde é o nome e onde é a a pountuação
                cabecalho = false;
            }

            int posiJog = -1;

            if (!UsouNome(nome, ref posiJog)) // se o nome ainda não foi utilizado
            {
                Jogador jog = new Jogador(nome, pontos); // cria-se um objeto da classe Jogador com nome e pontos do jogador ganhador
                vetorJogadores[qtsJogadores] = jog; // atribui-se o objeto ao vetor
                qtsJogadores++; // conta-se um jogador
                arqRanking.WriteLine(nome.PadRight(15, ' ') + " " + pontos); // escrevemos o nome e a respectiva pontuação
            }
            else //se o nome já foi utilizado
            {
                vetorJogadores[posiJog].PontosJogador += qtsPontos; // somamos a pontuação dessa jogada à pontuação da jogada anterior, já armazenada no vetor dos jogadores
                arqRanking.WriteLine(nome.PadRight(15, ' ') + " " + vetorJogadores[posiJog].PontosJogador); // escrevemos o nome a as somas da pontuação da rodada anterior e a dessa vez.
            }                                                     
        }


        bool UsouNome(string nomeJog, ref int posiJogadorEncontrado)
        {
            bool achou = false;
            for (int i = 0; i < qtsJogadores && !achou; i++) // percorremos o vetor dos jogadores já registrados
            {
                if (nomeJog == vetorJogadores[i].NomeJogador) //se o nome usado na jogada for igual a um já usado anteriormente
                {
                    achou = true; // quer dizer que o nome já existe
                    posiJogadorEncontrado = i; //devolvemos em qual indice do vetor de jogadores o nome foi encontrado
                } 
            }
            return achou; //retorna o bool se achou ou não o nome
        }

        void ResetarBotoes()
        {
            for (int i = 0; i < qtosBotoes; i++) // percorremos o vetor de botões pressionados
            { 
                quaisBotoes[i].Enabled = true; // habilitamos de volta
                quaisBotoes[i].BackColor = Color.White; // mudamos sua cor para a anterior
            }
            qtosBotoes = 0; //zeramos a quantidade de botões pressionados
        }

        void LimparDgv(DataGridView qualDgv)
        {
            for(int i = 0; i < vetor.QtosCaracteres; i++) // percorremos o vetor de caracteres
            {
                qualDgv.Rows[0].Cells[i].Value = "";  // para cada caracter(que está exibido no data grid view), limpamos o dgv
            }
            qualDgv.ColumnCount = 15; // voltamos para a posição normal do data grid view
        }

        int quantosSegundosFaltam = 60; // 60 segundos para o jogador adivinhar a palavra
        private void tmrTempo_Tick(object sender, EventArgs e)
        {
            lbTempo.Text = "Tempo Restante: " + quantosSegundosFaltam + " s"; // exibimos quantos segundos faltam
            quantosSegundosFaltam--;  // diminuimos 1 segundo a cada contagem

            if (quantosSegundosFaltam < 0) // se o timer chegou ao zero, ou seja, passou-se o tempo limite de 60 segundos
            {
                tmrTempo.Stop(); //paramos o timer
                tmrTempo.Enabled = false; // e também o desabilitamos
                GameOver(); // o jogador perde
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///--------------------------- MÉTODOS RELACIONADOS AS IMAGENS -----------------------------------------------///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///

        void ResetarImagens() // deixamos as imagens da forca invisíveis
        {
            picErro1.Visible = false;
            picAuxErro1.Visible = false;
            picErro2.Visible = false;
            picErro3.Visible = false;
            picErro4.Visible = false;
            picErro5.Visible = false;
            picErro6.Visible = false;
            picErro7.Visible = false;
            picErro8.Visible = false;
            gifMorto.Visible = false;
            picGanhou.Visible = false;
            picGanhou2.Visible = false;
        }

        void ExibirErrosNaForca(int nmrErro) // conforme o número de erros, exibimos a personagem na forca
        {
            switch (nmrErro) // utilizamos o switch para determinar qual imagem exibir conforme o número do erro
            {                // usamos também os comandos 'BringToFront()' para ajustar a imagem na forca
                case 1:
                    picErro1.Visible = true;
                    picAuxErro1.Visible = true;
                    picAuxErro1.BringToFront();
                    break;
                case 2:
                    picErro2.Visible = true;
                    break;
                case 3:
                    picErro3.Visible = true;
                    picErro3.BringToFront();
                    picAuxErro1.BringToFront();
                    break;
                case 4:
                    picErro4.Visible = true;
                    picErro4.BringToFront();
                    break;
                case 5:
                    picErro5.Visible = true;
                    break;
                case 6:
                    picErro6.Visible = true;
                    break;
                case 7:
                    picErro7.Visible = true;
                    break;
                case 8:
                    picErro8.Visible = true;
                    picErro8.BringToFront();
                    gifMorto.Visible = true;
                    break;
            }
        }

        void ImagensPerdeu() //exibimos as imagens da derrota
        {
            picErro1.Visible = true;
            picAuxErro1.Visible = true;
            picErro2.Visible = true;
            picErro3.Visible = true;
            picErro4.Visible = true;
            picErro5.Visible = true;
            picErro6.Visible = true;
            picErro7.Visible = true;
            picErro8.Visible = true;
            gifMorto.Visible = true;

            this.Refresh();  // atualizamos o form para exibir as imagens
            Application.DoEvents();
        }

        private void tmrAgora_Tick(object sender, EventArgs e)
        {
            lbData.Text = "Data: " + DateTime.Now.ToLongDateString(); // Exibe o dia em que o usuário está jogando
            lbHorario.Text = "Horário: " + DateTime.Now.ToString("hh:mm:ss"); // exibe o horário
        }

        void ImagensGanhou() //exibimos as imagens da vitória
        {
            picErro1.Visible = true;
            picAuxErro1.Visible = true;
            picErro2.Visible = true;
            picErro3.Visible = true;
            picErro4.Visible = true;
            picErro5.Visible = true;
            picErro6.Visible = true;
            picErro7.Visible = true;
            picGanhou.Visible = true;
            picGanhou2.Visible = true;

            this.Refresh(); // atualizamos o form para exibir as imagens
            Application.DoEvents();
        }

        //////////////////////////////////////////// --- Manutenção de dados --- /////////////////////////////////////////////////////////
        int ondeIncluir = -1;
        void AtualizarTela()
        {
            if (!vetor.EstaVazio)
            {
                int indice = vetor.PosicaoAtual;
                txtPalavra.Text = vetor[indice].PalavraUsada.Trim();
                txtDica.Text = vetor[indice].DicaUsada.Trim();
            }
            TestarBotoes();
        }

        private void TestarBotoes()
        {
            btnInicio.Enabled = true;
            btnAnterior.Enabled = true;
            btnProximo.Enabled = true;
            btnUltimo.Enabled = true;
            if (vetor.EstaNoInicio)
            {
                btnInicio.Enabled = false;
                btnAnterior.Enabled = false;
            }
            if (vetor.EstaNoFim)
            {
                btnProximo.Enabled = false;
                btnUltimo.Enabled = false;
            }
        }

        private void Limpar()
        {
            txtPalavra.Clear();
            txtDica.Clear();
        }

        private void Forca_FormClosing(object sender, FormClosingEventArgs e)
        {
            vetor.GravarDados(dlgAbrir.FileName);
        }

        private void tbCadastro_Enter(object sender, EventArgs e)
        {
            if (!vetor.EstaVazio)
            {
                vetor.PosicionarNoPrimeiro();
                AtualizarTela();
            }
        }
        private void txtPalavra_Leave(object sender, EventArgs e)
        {
            if (vetor.SituacaoAtual == Situacao.pesquisando)
            {
                if (!vetor.EstaVazio)
                {
                    int indice = -1;
                    var procurado = new PalavraDica(txtPalavra.Text.ToLower(), "");
                    if (vetor.Existe(procurado, ref indice))
                    {
                        vetor.PosicaoAtual = indice;
                        AtualizarTela();
                        stlbMensagem.Text = "Palavra encontrada na posição " + indice;
                    }
                    else
                    {
                        MessageBox.Show("A palavra pesquisada não existe no jogo");
                        vetor.PosicionarNoPrimeiro();
                        AtualizarTela();
                    }
                    txtPalavra.ReadOnly = true;
                    vetor.SituacaoAtual = Situacao.navegando;
                }
                else
                    MessageBox.Show("Não há nenhuma palavra no jogo!\nAbra um arquivo ou as adicione!");
            }

         else
            if (vetor.SituacaoAtual == Situacao.incluindo)
            {
                PalavraDica novaPalavra = new PalavraDica(txtPalavra.Text.ToLower(), "");
                if(vetor.Existe(novaPalavra, ref ondeIncluir))
                {
                    MessageBox.Show("Palavra repetida, inclusão cancelada");
                    Limpar();
                    vetor.SituacaoAtual = Situacao.navegando;
                    txtPalavra.ReadOnly = true;
                    txtDica.ReadOnly = true;
                }
                else
                {
                    stlbMensagem.Text = "Digite os outros campos, após isso pressione [Salvar]";
                    btnSalvar.Enabled = true;
                    txtDica.Focus();
                }
            }
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            vetor.PosicionarNoPrimeiro();
            AtualizarTela();
        }


        private void btnAnterior_Click(object sender, EventArgs e)
        {
            vetor.RetrocederPosicao();
            AtualizarTela();
        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
            vetor.AvancarPosicao();
            AtualizarTela();
        }

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            if (!vetor.EstaVazio)
            {
                vetor.SituacaoAtual = Situacao.pesquisando;
                txtPalavra.ReadOnly = false;
                Limpar();
                txtPalavra.Focus();
                stlbMensagem.Text = "Digite a palavra procurada";

            }
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            Limpar();
            vetor.SituacaoAtual = Situacao.incluindo;
            txtPalavra.ReadOnly = false;
            txtDica.ReadOnly = false;
            txtPalavra.Focus();
            stlbMensagem.Text = "Digite a palavra e a dica nova";
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            vetor.PosicionarNoUltimo();
            AtualizarTela();
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void btnCancelar_Click(object sender, EventArgs e)
        {
            vetor.SituacaoAtual = Situacao.navegando;
            AtualizarTela();
            btnSalvar.Enabled = false;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja excluir esse registro?", "Exclusão", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                vetor.Excluir(vetor.PosicaoAtual);
                if (vetor.PosicaoAtual > vetor.Tamanho)
                    vetor.PosicionarNoUltimo();
                AtualizarTela();

            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            vetor.SituacaoAtual = Situacao.editando;
            txtPalavra.ReadOnly = false;
            txtDica.ReadOnly = false;
            btnSalvar.Enabled = true;
            stlbMensagem.Text = "Edite os campos desejados e pressione [Salvar]";
        }


        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if(vetor.SituacaoAtual == Situacao.incluindo)
            {
                if (txtDica.Text != "")
                {
                    var novoDesafio = new PalavraDica(txtPalavra.Text.ToLower().PadRight(15, ' '), txtDica.Text.PadRight(100, ' '));
                    vetor.Incluir(novoDesafio, ondeIncluir);
                    vetor.PosicaoAtual = ondeIncluir;
                }
                else
                    MessageBox.Show("Digite uma dica para sua palavra!");
            }
            else
                if(vetor.SituacaoAtual == Situacao.editando)
                {
                     vetor[vetor.PosicaoAtual] = new PalavraDica(txtPalavra.Text.ToLower().PadRight(15,' '), txtDica.Text.PadRight(100, ' '));
                }
            AtualizarTela();
            vetor.SituacaoAtual = Situacao.navegando;
            btnSalvar.Enabled = false;
            txtPalavra.ReadOnly = true;
            txtDica.ReadOnly = true;
        }
    }
}
