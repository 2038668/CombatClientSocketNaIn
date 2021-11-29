using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CombatClientSocketNaIn.Classes;


namespace CombatClientSocketNaIn
{
    public partial class frmClienSocketNain : Form
    {
        Random m_r;
        Elfe m_elfe;
        Nain m_nain;
        public frmClienSocketNain()
        {
            InitializeComponent();
            m_r = new Random();
            Reset();
            btnReset.Enabled = false;
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        void Reset()
        {
            m_nain = new Nain(m_r.Next(10, 20), m_r.Next(2, 6), m_r.Next(0, 3));
            picNain.Image = m_nain.Avatar;
            lblVieNain.Text = "Vie: " + m_nain.Vie.ToString(); ;
            lblForceNain.Text = "Force: " + m_nain.Force.ToString();
            lblArmeNain.Text = "Arme: " + m_nain.Arme;

            m_elfe = new Elfe(1, 0, 0);
            picElfe.Image = m_elfe.Avatar;
            lblVieElfe.Text = "Vie: " + m_elfe.Vie.ToString();
            lblForceElfe.Text = "Force: " + m_elfe.Force.ToString();
            lblSortElfe.Text = "Sort: " + m_elfe.Sort.ToString();
        }
        void AfficheStatNain(int vie,int force, string arme)
        {
            lblVieNain.Text = "Vie: " + vie.ToString();
            lblForceNain.Text = "Force: " + force.ToString();
            lblArmeNain.Text = "Arme: " + arme;

            this.Update(); // pour s'assurer de l'affichage via le thread
        }
        void AfficheStatElfe(int vie, int force, int sort)
        {
            lblVieElfe.Text = "Vie: " + vie.ToString();
            lblForceElfe.Text = "Force: " + force.ToString();
            lblSortElfe.Text = "Sort: " + sort.ToString();

            this.Update(); // pour s'assurer de l'affichage via le thread
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            btnFrappe.Enabled = true;
            Reset();
        }

        private void btnFrappe_Click(object sender, EventArgs e)
        {
            btnFrappe.Enabled = false;
            btnReset.Enabled = true;
            string reponse;
            string[] ts;
            int nbOctetReception;
            byte[] tByteReceptionClient = new byte[100];
            ASCIIEncoding textByte = new ASCIIEncoding();
            byte[] tByteEnvoie;
            try
            {
                Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);
                client.Connect(IPAddress.Parse("127.0.0.1"), 8888);
                MessageBox.Show("assurez-vous que le serveur est démarré et en attente d'un client");
                if (client.Connected)
                {
                    string n = m_nain.Vie + ";" + m_nain.Force + ";" + m_nain.Arme;
                    MessageBox.Show("Client: \r\nTransmet..." + n);
                    tByteEnvoie = textByte.GetBytes(n);

                    //transmission
                    client.Send(tByteEnvoie);
                    Thread.Sleep(500);
                    //reception
                    MessageBox.Show("Client: réception de données serveur");
                    nbOctetReception = client.Receive(tByteReceptionClient);
                    reponse = Encoding.ASCII.GetString(tByteReceptionClient);
                    MessageBox.Show("\r\nReception..." + reponse);
                    //split sur le string de reception pour afficher les nouvelles stats nain/elfe
                    ts = reponse.Split(';');
                    //afficher les resultats
                    m_nain.Vie = Int16.Parse(ts[0]);
                    int ptvien = m_nain.Vie;
                    m_nain.Force = Int16.Parse(ts[1]);
                    int ptforcen = m_nain.Force;
                    m_nain.Arme = ts[2];
                    string sortearmen = m_nain.Arme;
                    AfficheStatNain(ptvien, ptforcen, sortearmen);
                    //elfe
                    m_elfe.Vie = Int16.Parse(ts[3]);
                    int ptviee = m_elfe.Vie;
                    m_elfe.Force = Int16.Parse(ts[4]);
                    int ptforcee = m_elfe.Force;
                    m_elfe.Sort = Int16.Parse(ts[5]);
                    int ptsort = m_elfe.Sort;
                    AfficheStatElfe(ptviee, ptforcee, ptsort);
                    //afficher le gagnant
                    if (m_nain.Vie > 0)
                    {
                        lblWinner.Text ="Nain Gagne";
                    }
                    else if (m_elfe.Vie > 0)
                    {
                        lblWinner.Text = "Elfe gagne!!";
                    }

                }
                client.Close();
                txtStatus.Text += Environment.NewLine + "Déconnecté";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnFrappe.Enabled = true;
        }

private void frmClienSocketNain_Load(object sender, EventArgs e)
        {

        }
    }
}
