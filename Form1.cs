using jakaApi;
using jkType;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIM_JAKA
{
    public partial class Form1 : Form
    {
        private int robotHandle = -1;
        private string ipRobot = "10.5.5.100";
        private double[] posicionHome = new double[6] { 1.090, 1.158, 1.99, 1.517, -1.593, 3.930 };
        private readonly double[] purgaposicion = new double[6] { -280.27, -144.81, 91.93, 3.08, 0.00, -1.49 };
        private readonly System.Threading.AutoResetEvent semaforoTornillo = new AutoResetEvent(false);
        private readonly List<Herramienta> listaHerramientas = new List<Herramienta>();
        private readonly List<Trayectoria> historialTrayectorias = new List<Trayectoria>();
        private readonly Punto[] puntosInterfazFijos = new Punto[10];

        private bool enMovimiento = false;
        private bool modoEdicion = false;
        private int indiceTrayectoriaAEditar = -1;
        private int guardado = -1;
        private bool secuenciaActiva = false;
        private bool estaEjecutando = false;
        private bool estadoAnteriorBoton = false;
        private int punto = -1;
        private double velocidadPorcentaje = 20.0;

        private readonly AutoResetEvent semaforoAtornillado = new AutoResetEvent(false);
        private bool esperandoDI1 = false;
        private bool ConTornillo = false;
        private bool esperandoAtornillado = false;
        private int indexDO = 0;

        private bool Atornillado = false;
        private bool ejecucionExitosa = false;
        private Button[] luzEntradas;  private Button[] luzSalidas; private Label[] luzEntradasA; private Label[] luzSalidasA;

        public enum EstadoPaso { Inactivo, EnProceso, Completado, Error }
        public Form1()
        {
            InitializeComponent();
            btnInterruptor.Enabled = false;
            btnHabilitar.Enabled = false;
            pnlAjustes.Visible = false;  pnlTrayectorias.Visible = false;
            pnlIO.Visible = false;  pnlControl.Visible = false;

            for (int i = 0; i < 10; i++) { puntosInterfazFijos[i] = new Punto { Velocidad = 15.0, OffsetMm = 0.0, Herramienta = "Ninguna" }; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnConectar.Enabled = false; btnConectar.Text = "Estabilizando"; btnConectar.BackColor = Color.SlateBlue;

            Task.Delay(2000).ContinueWith(t =>
            {
                this.Invoke((MethodInvoker)delegate
            { btnConectar.Enabled = true; btnConectar.Text = "Conectar al Robot"; btnConectar.BackColor = Color.CornflowerBlue; });
            });

            listaHerramientas.Add(new Herramienta("Pluma", 1, 1.104, -0.172, 133.778, 0, 0, 0));
            listaHerramientas.Add(new Herramienta("Herramienta", 2, -5.631, -4.817, 132.966, 0, 0, 0));

            luzEntradas = new Button[10] { lzDI1, lzDI2, lzDI3, lzDI4, lzDI5, lzDI6, lzDI7, lzDI8, lzDI9, lzDI10 };
            luzSalidas = new Button[10] { lzDO1, lzDO2, lzDO3, lzDO4, lzDO5, lzDO6, lzDO7, lzDO8, lzDO9, lzDO10 };
            luzEntradasA = new Label[10] { txtAI1, txtAI2, txtAI3, txtAI4, txtAI5, txtAI6, txtAI7, txtAI8, txtAI9, txtAI10 };
            luzSalidasA = new Label[10] { txtAO1, txtAO2, txtAO3, txtAO4, txtAO5, txtAO6, txtAO7, txtAO8, txtAO9, txtAO10 };

            cmbTools.DataSource = listaHerramientas;
            cmbTools.DisplayMember = "Nombre";
            cmbTools.ValueMember = "DatosCalibracion";
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (robotHandle != -1)
            {
                TimerRep.Stop(); {
                    Task.Run(() => { jakaAPI.disable_robot(ref robotHandle);});
                    Task.Run(() => { jakaAPI.power_off(ref robotHandle); jakaAPI.destory_handler(ref robotHandle); });
                };}}
        private bool RobotListo()
        {
            if (lblStatusConexion.Text == "Desconectado" || lblStatus.Text == "Apagado" || lblHabi.Text == "Deshabilitado")
            { MessageBox.Show("Verifica la conexión, el encendido y la habilitación del robot.", "Error al Continuar."); return false; }
            return true;
        }
        private void TimerRep_Tick(object sender, EventArgs e)
        {
            if (robotHandle == -1) return;
            if (pnlIO.Visible == false)
            {
                int indexDI1 = 0; bool estadoDI1 = false;
                int indexDI2 = 1; bool estadoDI2 = false;
                int indexDI3 = 2; bool estadoDI3 = false;
                int indexDI4 = 3; bool estadoDI4 = false;

                int res1BI = jakaAPI.get_digital_input(ref robotHandle, JKTYPE.IOType.IO_CABINET, indexDI1, ref estadoDI1);
                bool botonPresionadoAhora = (res1BI == 0 && estadoDI1 == true);

                if (botonPresionadoAhora && !estadoAnteriorBoton)
                {
                    if (esperandoDI1) { esperandoDI1 = false; }
                    else if (!estaEjecutando) { _ = IniciarSecuenciaReproducir(); }
                }
                estadoAnteriorBoton = botonPresionadoAhora;

                int res2BA = jakaAPI.get_digital_input(ref robotHandle, JKTYPE.IOType.IO_CABINET, indexDI2, ref estadoDI2);
                bool botonTornillo = (res2BA == 0 && estadoDI2 == true);
                if (botonTornillo) { ConTornillo = true; Indicadores(lzConTornillo, EstadoPaso.Completado); 
                    jakaAPI.set_digital_output(ref robotHandle, JKTYPE.IOType.IO_CABINET, indexDO, false); semaforoTornillo.Set(); }
                
                if (secuenciaActiva && esperandoAtornillado)
                {
                    int res3BAB = jakaAPI.get_digital_input(ref robotHandle, JKTYPE.IOType.IO_CABINET, indexDI3, ref estadoDI3);
                    bool bAtornilladoBn = (res3BAB == 0 && estadoDI3 == true);

                    int res4BAM = jakaAPI.get_digital_input(ref robotHandle, JKTYPE.IOType.IO_CABINET, indexDI4, ref estadoDI4);
                    bool bAtornilladoMl = (res4BAM == 0 && estadoDI4 == true);

                    if (bAtornilladoBn)
                    {
                        Atornillado = true; esperandoAtornillado = false;
                        Indicadores(lzAtornillado, EstadoPaso.Completado); semaforoAtornillado.Set();
                    }
                    if (bAtornilladoMl)
                    {
                        Atornillado = false; esperandoAtornillado = false;
                        Indicadores(lzAtornillado, EstadoPaso.Error); Indicadores(lzConTornillo, EstadoPaso.Error); semaforoAtornillado.Set();
                    }
                }
            }
        }
        private void TimerIO_Tick(object sender, EventArgs e)
        {

            if (robotHandle == -1) return;
            if (lblStatusConexion.Text == "Conectado")
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        bool estadoPinED = false;
                        jakaAPI.get_digital_input(ref robotHandle, JKTYPE.IOType.IO_CABINET, i, ref estadoPinED);
                        luzEntradas[i].BackColor = estadoPinED ? Color.LimeGreen : SystemColors.ScrollBar;
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        bool estadoPinSD = false;
                        jakaAPI.get_digital_output(ref robotHandle, JKTYPE.IOType.IO_CABINET, i, ref estadoPinSD);
                        luzSalidas[i].BackColor = estadoPinSD ? Color.LimeGreen : SystemColors.ScrollBar;

                    }
                    for (int i = 0; i < 10; i++)
                    {
                        float estadoPinEA = 0.0f;
                        jakaAPI.get_analog_input(ref robotHandle, JKTYPE.IOType.IO_CABINET, i, ref estadoPinEA);
                        luzEntradasA[i].Text = estadoPinEA.ToString("F2") + "V";
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        float estadoPinSA = 0.0f;
                        jakaAPI.get_analog_output(ref robotHandle, JKTYPE.IOType.IO_CABINET, i, ref estadoPinSA);
                        luzSalidasA[i].Text = estadoPinSA.ToString("F2") + "V";
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error de comunicacion I / O:" + ex.Message); }
            }
            else
            {
                TimerIO.Stop(); this.Text = "HMI JAKA - Desconectado Estados I/O";
            }
        }
        private void TimerMon_Tick(object sender, EventArgs e)
        {
            if (robotHandle == -1) return;

            JKTYPE.JointValue posicionActualStruct = new JKTYPE.JointValue();
            int res = jakaAPI.get_joint_position(ref robotHandle, ref posicionActualStruct);
            if (res == 0)
            {
                double[] articulacionesActuales = posicionActualStruct.jVal;
                if (VerificarSiEstaEnHome(articulacionesActuales, posicionHome))
                { lblEstadoHome.Text = "Robot en HOME"; lzHome.BackColor = Color.Green; }
                else
                { lblEstadoHome.Text = "Fuera de HOME"; lzHome.BackColor = Color.DarkRed; }
            }

            if (robotHandle == -1) { RobotDesconectado(); return; }

            JKTYPE.RobotState estado = default;
            int resultado = jakaAPI.get_robot_state(ref robotHandle, ref estado);

            if (resultado != 0) { RobotDesconectado(); return; }
            lblStatusConexion.Text = "Conectado"; lblStatusConexion.ForeColor = Color.DarkGreen; btnInterruptor.Enabled = true;

            if (estado.poweredOn == 1)
            { lblStatus.Text = "Encendido"; lzStatus.BackColor = Color.Green; btnInterruptor.Text = "Apagar"; btnHabilitar.Enabled = true; }

            else
            {
                if (btnInterruptor.Text == "Encendiendo...")
                { lblStatus.Text = "Encendiendo..."; lzStatus.BackColor = Color.Gold; btnInterruptor.Enabled = false; btnHabilitar.Enabled = false; }
                else
                {
                    lblStatus.Text = "Apagado"; lzStatus.BackColor = Color.DarkRed; btnInterruptor.Text = "Encender"; btnHabilitar.Enabled = false;
                    lblHabi.Text = "Deshabilitado"; lzHabi.BackColor = Color.DarkRed; btnHabilitar.Text = "Habilitar"; return;
                }
            }
            if (estado.servoEnabled == 1)
            { lblHabi.Text = "Habilitado"; lzHabi.BackColor = Color.Green; btnHabilitar.Text = "Deshabilitar"; btnHabilitar.Enabled = true; }
            else
            {
                if (btnHabilitar.Text == "Habilitando...")
                { lblHabi.Text = "Habilitando..."; lzHabi.BackColor = Color.Gold; btnHabilitar.Enabled = false; }
                else { lblHabi.Text = "Deshabilitado"; lzHabi.BackColor = Color.DarkRed; btnHabilitar.Text = "Habilitar"; btnHabilitar.Enabled = (estado.poweredOn == 1); }
            }

        }

        private void BtnConectar_Click(object sender, EventArgs e)
        {
            if (btnConectar.Text == "Conectar al Robot")
            {
                this.Text = "Intentando conectar... ";
                btnConectar.Enabled = false;

                Task.Run(() =>
                {
                    int resultado = jakaAPI.create_handler(ipRobot, ref robotHandle);

                    this.Invoke((MethodInvoker)delegate
                    {
                        btnConectar.Enabled = true;
                        if (resultado == 0)
                        {
                            this.Text = "HMI JAKA - ¡CONECTADO! ";
                            btnConectar.BackColor = Color.LightGreen;
                            btnConectar.Text = "Conectado";
                            TimerRep.Start();
                            TimerMon.Start();
                            lblStatusConexion.Text = "Conectado";
                            lblStatusConexion.ForeColor = Color.DarkGreen;
                            Task.Delay(1000).ContinueWith(t =>
                            { this.Invoke((MethodInvoker)delegate { btnInterruptor.Enabled = true; }); });
                        }
                        else
                        {
                            this.Text = "HMI JAKA - DESCONECTADO (Error: " + resultado + ")";
                            btnConectar.BackColor = Color.Coral; btnConectar.Text = "FALLO CONEXION";
                        }
                    });
                });
            }
            else
            {
                if (lblStatus.Text == "Encendido") { MessageBox.Show("Primero apagar robot. ", "Error al Desconectar."); }
                else
                {
                    TimerRep.Stop();

                    Task.Run(() => { jakaAPI.destory_handler(ref robotHandle); });
                    robotHandle = -1;

                    this.Text = "HIM JAKA - Desconectado";
                    btnConectar.Text = "Conectar al Robot"; btnConectar.BackColor = Color.CornflowerBlue;
                    lblStatusConexion.Text = "Desconectado"; lblStatusConexion.ForeColor = Color.DarkRed;
                    lblEstadoHome.Text = "Desconocido"; lzHome.BackColor = Color.DarkRed;
                }
            }
        }
        private void BtnEncender_Click(object sender, EventArgs e)
        {
            if (btnHabilitar.Text == "Deshabilitar") { MessageBox.Show("Primero deshabilita el robot.", "Error al Apagar."); }

            else if (lblStatusConexion.Text == "Desconectado") { MessageBox.Show("Primero conecta el robot.", "Error al Encender."); }

            if (btnInterruptor.Text == "Encender")
            {
                btnInterruptor.Text = "Encendiendo...";
                lblStatus.Text = "Encendiendo..."; lzStatus.BackColor = Color.Gold;
                btnInterruptor.Enabled = false; Task.Run(() => { jakaAPI.power_on(ref robotHandle); });
            }
            else
            {
                btnInterruptor.Text = "Apagando...";
                lblStatus.Text = "Apagando..."; lzStatus.BackColor = Color.Gold;
                btnHabilitar.Enabled = false; Task.Run(() => { jakaAPI.power_off(ref robotHandle); });
            }

        }
        private void BtnHabilitar_Click(object sender, EventArgs e)
        {
            if (lblStatus.Text == "Apagado") { MessageBox.Show("Primero enciende el Robot.", "Error al Habilitar."); }

            if (btnHabilitar.Text == "Habilitar")
            {
                btnHabilitar.Text = "Habilitando..."; lblHabi.Text = "Habilitando..."; btnHabilitar.Enabled = false; lzHabi.BackColor = Color.Gold;
                Task.Run(() => { jakaAPI.enable_robot(ref robotHandle); });
            }

            else
            {
                btnHabilitar.Text = "Deshabilitando..."; lblHabi.Text = "Deshabilitando..."; btnHabilitar.Enabled = false; lzHabi.BackColor = Color.Gold;
                Task.Run(() => { jakaAPI.disable_robot(ref robotHandle); });
            }
        }
        private async Task IniciarSecuenciaReproducir()
        {
            if (!RobotListo()) return;
            if (estaEjecutando) return;


            if (cmbTools.SelectedItem == null || cmbLista.SelectedIndex == -1) { MessageBox.Show("Asegúrate de seleccionar una trayectoria.", "Error al Comenzar."); return; }

            JKTYPE.JointValue posicionActualStruct = new JKTYPE.JointValue();
            int res = jakaAPI.get_joint_position(ref robotHandle, ref posicionActualStruct);
            if (res != 0) { MessageBox.Show("No se pudo obtener la posicion actual del robot.", "Error de Conexion"); return; }

            if (!VerificarSiEstaEnHome(posicionActualStruct.jVal, posicionHome)) { MessageBox.Show("Mandar robot a Home antes de reproducir.", "Error al Comenzar."); return; }

            Herramienta herramientaActiva = (Herramienta)cmbTools.SelectedItem;
            JKTYPE.CartesianPose poseTCP = herramientaActiva.DatosCalibracion;
            int resultadoTCP = jakaAPI.set_tool_data(ref robotHandle, herramientaActiva.Id, ref poseTCP, herramientaActiva.Nombre);
            int resultadoActivarTool = jakaAPI.set_tool_id(ref robotHandle, herramientaActiva.Id);

            if (resultadoTCP != 0 || resultadoActivarTool != 0) { MessageBox.Show("Error al configurar TCP, trayectoria cancelada", "Error al Encender."); return; }

            int indiceSeleccionado = cmbLista.SelectedIndex;
            Trayectoria trayectoriaA_Ejecutar = historialTrayectorias[indiceSeleccionado];
            int pasosTrayectoria = trayectoriaA_Ejecutar.Pasos.Count;

            JKTYPE.JointValue jValHome = new JKTYPE.JointValue() { jVal = posicionHome };
            estaEjecutando = true; secuenciaActiva = true; btnReproducir.Enabled = false; btnHome.Enabled = false;

            this.Text = "HMI JAKA - Ejecutando Trayectoria..."; lzStTray.BackColor = Color.Gold; lblStTray.Text = "Ejecutando Trayectoria";
            try
            {
                await Task.Run(() =>
                {
                    if (!ConTornillo)
                    { this.Invoke(new Action(() => { this.Text = "HMI JAKA - Esperando Tornillo"; }));
                        jakaAPI.set_digital_output(ref robotHandle, JKTYPE.IOType.IO_CABINET, indexDO, true);
                        Indicadores(lzConTornillo, EstadoPaso.EnProceso); semaforoTornillo.Reset(); semaforoTornillo.WaitOne(); }

                    for (int i = 0; i < pasosTrayectoria; i++)
                    {
                        Punto punto = trayectoriaA_Ejecutar.Pasos[i];
                        this.Invoke((MethodInvoker)delegate { this.Text = $"Viajando al paso {i + 1} de {trayectoriaA_Ejecutar.Pasos.Count}..."; }); Indicadores(lzInició, EstadoPaso.Completado);

                        double posX = punto.PoseFinal[0];
                        double posY = punto.PoseFinal[1];
                        double posZ = punto.PoseFinal[2];
                        double rx = punto.PoseFinal[3];
                        double ry = punto.PoseFinal[4];
                        double rz = punto.PoseFinal[5];

                        JKTYPE.CartesianPose posePurga = JKTYPE.CartesianPose.withRadians(purgaposicion[0], purgaposicion[1], purgaposicion[2], purgaposicion[3], purgaposicion[4], purgaposicion[5]);

                        JKTYPE.CartesianPose coordenadasDestinoFinal = JKTYPE.CartesianPose.withRadians(punto.PoseFinal[0], punto.PoseFinal[1], punto.PoseFinal[2], punto.PoseFinal[3], punto.PoseFinal[4], punto.PoseFinal[5]);

                        // string datos = $"{punto.PoseFinal[0]},{punto.PoseFinal[1]}, {punto.PoseFinal[2]}, {punto.PoseFinal[3]}, {punto.PoseFinal[4]},{punto.PoseFinal[5]}";  MessageBox.Show(datos);
                        if (punto.OffsetMm > 0)
                        {
                            double z_x = Math.Cos(rz) * Math.Sin(ry) * Math.Cos(rx) + Math.Sin(rz) * Math.Sin(rx);
                            double z_y = Math.Sin(rz) * Math.Sin(ry) * Math.Cos(rx) - Math.Cos(rz) * Math.Sin(rx);
                            double z_z = Math.Cos(ry) * Math.Cos(rx);

                            double approachX = posX - (punto.OffsetMm * z_x);
                            double approachY = posY - (punto.OffsetMm * z_y);
                            double approachZ = posZ - (punto.OffsetMm * z_z);

                            JKTYPE.CartesianPose poseAproximacion = JKTYPE.CartesianPose.withRadians(approachX, approachY, approachZ, rx, ry, rz);

                            jakaAPI.linear_move(ref robotHandle, ref poseAproximacion, 0, true, punto.VelNorm);
                            if (!ConTornillo)
                            {
                                this.Invoke((MethodInvoker)delegate { this.Text = $"Paso {i + 1}: Coloque tornillo y presione boton..."; });
                                jakaAPI.set_digital_output(ref robotHandle, JKTYPE.IOType.IO_CABINET, indexDO, true); esperandoAtornillado= true;
                                Indicadores(lzConTornillo, EstadoPaso.EnProceso); semaforoTornillo.Reset(); semaforoTornillo.WaitOne(); Indicadores(lzLlegoAPunto, EstadoPaso.Completado);
                            }
                            jakaAPI.linear_move(ref robotHandle, ref coordenadasDestinoFinal, 0, true, punto.Velocidad);
                            Indicadores(lzLlegoAPunto, EstadoPaso.Completado);

                            jakaAPI.linear_move(ref robotHandle, ref poseAproximacion, 0, true, punto.Velocidad);
                            if (i != pasosTrayectoria - 1) { Indicadores(lzConTornillo, EstadoPaso.EnProceso);}
                                this.Invoke((MethodInvoker)delegate
                                { this.Text = $"⚠️ Paso {i + 1}: - Inspeccion..."; Indicadores(lzAtornillado, EstadoPaso.EnProceso);  });
                            
                            semaforoAtornillado.Reset(); esperandoAtornillado = true; semaforoAtornillado.WaitOne();

                            if (Atornillado == false)
                            {
                                this.Invoke((MethodInvoker)delegate { this.Text = $"⚠️ Paso {i + 1}: - Inspeccion... Marque Atornillado."; });
                                ConTornillo = false; jakaAPI.linear_move(ref robotHandle, ref posePurga, 0, true, 80); Thread.Sleep(600); jakaAPI.joint_move(ref robotHandle, ref jValHome, JKTYPE.MoveMode.ABS, true, 0.4);
                                secuenciaActiva = false; estaEjecutando = false; ResetearLuces(); return;
                            }

                        }
                        else { jakaAPI.linear_move(ref robotHandle, ref coordenadasDestinoFinal, 0, true, punto.VelNorm); }

                        ConTornillo = false;
                        this.Invoke((MethodInvoker)delegate
                        { this.Text = $"⚠️ Robot en paso {i + 1} - Esperando validación..."; });
                        
                        this.Invoke((MethodInvoker)delegate { this.Text = "HMI JAKA - Esperando Tornillo"; });
                        Indicadores(lzLlegoAPunto, EstadoPaso.EnProceso);
                        this.Invoke((MethodInvoker)delegate { this.Text = "HMI JAKA - Siguiente Punto";});
                            
                    }
                    Indicadores(lzConTornillo, EstadoPaso.Inactivo);
                    this.Invoke((MethodInvoker)delegate { this.Text = "HMI JAKA - Regresando a HOME."; });
                    jakaAPI.joint_move(ref robotHandle, ref jValHome, JKTYPE.MoveMode.ABS, true, 0.4);
                    Indicadores(lzLlegoAPunto, EstadoPaso.Completado);
                    ejecucionExitosa = true;
                });
            }
            catch (Exception ex) { this.Invoke((MethodInvoker)delegate { MessageBox.Show($"Error en ejecución: {ex.Message}"); }); Indicadores(lzLlegoAPunto, EstadoPaso.Error); }

            finally
            {
                secuenciaActiva = false; estaEjecutando = false; btnReproducir.Enabled = true; btnHome.Enabled = true; esperandoDI1 = true;
                this.Text = "HMI JAKA - Listo";
                if (ejecucionExitosa)
                {
                    lzStTray.BackColor = Color.Green; lblStTray.Text = "Trayectoria terminada";
                    await Task.Delay(1700);
                    ResetearLuces();
                    lzStTray.BackColor = Color.Gray; lblStTray.Text = "Sin trayectoria ";
                }
                else { lzStTray.BackColor = Color.DarkRed; lblStTray.Text = "Desconocido"; }
            }
        }
        private async void BtnReproducir_Click(object sender, EventArgs e) { if (!estaEjecutando) { await IniciarSecuenciaReproducir(); } estadoAnteriorBoton = true; }

        private void CapturarPuntoFila(int indicePunto, ComboBox cmbVel, ComboBox cmbOff, ComboBox cmbVN, Button lzPto)
        {
            if (!RobotListo()) return;

            if (modoEdicion)
            {
                DialogResult advertencia = MessageBox.Show(
                    $"Estás en Modo Edición. Al continuar se SOBREESCRIBIRÁN por completo los datos del Punto {indicePunto + 1}.\n Asegurate de cambiarlo en la posicion deseada. \n\n¿Estás seguro de capturar estos nuevos valores?",
                    "ATENCION!!  Modificación de Punto", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (advertencia == DialogResult.No) return;
            }

            JKTYPE.CartesianPose posicionRobot = new JKTYPE.CartesianPose();
            int res = jakaAPI.get_tcp_position(ref robotHandle, ref posicionRobot);

            if (res == 0)
            {
                if (cmbVN.Text == "")
                { MessageBox.Show($"Por favor, asigna primero la velocidad para el punto {indicePunto + 1}.", "Requisitos faltantes."); return; }

                if (cmbVel.Text == "")
                { MessageBox.Show($"Por favor, asigna primero la velocidad para el offset del punto {indicePunto + 1}.", "Requisitos faltantes."); return; }

                double offsetValue = 0.0;
                if (cmbOff.Text != null)
                { double.TryParse(cmbOff.Text, out offsetValue); }

                Punto p = puntosInterfazFijos[indicePunto];
                p.PoseFinal[0] = posicionRobot.tran.x;
                p.PoseFinal[1] = posicionRobot.tran.y;
                p.PoseFinal[2] = posicionRobot.tran.z;
                p.PoseFinal[3] = posicionRobot.rpy.rx;
                p.PoseFinal[4] = posicionRobot.rpy.ry;
                p.PoseFinal[5] = posicionRobot.rpy.rz;

                p.Velocidad = Convert.ToDouble(cmbVel.Text);
                p.OffsetMm = offsetValue;
                p.Herramienta = cmbTools.Text;
                p.VelNorm = Convert.ToDouble(cmbVN.Text);
                lzPto.BackColor = Color.Green;

                MessageBox.Show($"        ¡Punto {indicePunto + 1} capturado!\nVelocidad Trayecto: {p.VelNorm} mm/s\nOffset: {p.OffsetMm} mm\n Velocidad de off set: {p.Velocidad} mm/s", "Punto Guardado.");
                /*   JKTYPE.CartesianPose poseActualStruct = new JKTYPE.CartesianPose(); int iii= jakaAPI.get_tcp_position(ref robotHandle, ref poseActualStruct);
                if (iii == 0)
                {string datosCartesianos = $"public readonly double[] coordenadas = new double[6] {{" +
                   $" {poseActualStruct.tran.x}, {poseActualStruct.tran.y}, {poseActualStruct.tran.z}, " +
                    $" {poseActualStruct.rpy.rx}, {poseActualStruct.rpy.ry}, {poseActualStruct.rpy.rz} }};";
                    MessageBox.Show(datosCartesianos, "pUNTO CARTESIANO EXTRAIDO");}*/
            }
            else
            { MessageBox.Show("Error al leer la posición actual del brazo.", "Error."); }
        }
        private void btnCapturar_Click(object sender, EventArgs e)
        {
            if (punto == -1) return;

            ComboBox[] combosVel = new ComboBox[] { cmbVTP1, cmbVTP2, cmbVTP3, cmbVTP4, cmbVTP5, cmbVTP6, cmbVTP7, cmbVTP8, cmbVTP9, cmbVTP10 };
            ComboBox[] combosOff = new ComboBox[] { cmbMOP1, cmbMOP2, cmbMOP3, cmbMOP4, cmbMOP5, cmbMOP6, cmbMOP7, cmbMOP8, cmbMOP9, cmbMOP10 };
            ComboBox[] combosVN = new ComboBox[] { cmbVN1, cmbVN2, cmbVN3, cmbVN4, cmbVN5, cmbVN6, cmbVN7, cmbVN8, cmbVN9, cmbVN10 };
            Button[] lzPto = new Button[] { lzP1, lzP2, lzP3, lzP4, lzP5, lzP6, lzP7, lzP8, lzP9, lzP10 };
            CapturarPuntoFila(punto, combosVel[punto], combosOff[punto], combosVN[punto], lzPto[punto]);
        }
        private void BtnCapPto1_Click(object sender, EventArgs e) { pnlControlPunto (0); }
        private void BtnCapPto2_Click(object sender, EventArgs e) { pnlControlPunto(1); }
        private void BtnCapPto3_Click(object sender, EventArgs e) { pnlControlPunto(2); }
        private void BtnCapPto4_Click(object sender, EventArgs e) { pnlControlPunto(3); }
        private void BtnCapPto5_Click(object sender, EventArgs e) { pnlControlPunto(4); }
        private void BtnCapPto6_Click(object sender, EventArgs e) { pnlControlPunto(5); }
        private void BtnCapPto7_Click(object sender, EventArgs e) { pnlControlPunto(6); }
        private void BtnCapPto8_Click(object sender, EventArgs e) { pnlControlPunto(7); }
        private void BtnCapPto9_Click(object sender, EventArgs e) { pnlControlPunto(8); }
        private void BtnCapPto10_Click(object sender, EventArgs e) { pnlControlPunto(9); }

        private void MoverAPuntoGuardado(int indicePunto)
        {
            if (!RobotListo()) return;

            Punto p = puntosInterfazFijos[indicePunto];
            if (p.PoseFinal[0] == 0 && p.PoseFinal[1] == 0 && p.PoseFinal[2] == 0)
            { MessageBox.Show("Este punto no cuenta con coordenadas válidas registradas.", "Falta Captura"); return; }

            DialogResult respuesta = MessageBox.Show($"¿Deseas desplazar automáticamente al robot hacia la posición original del Punto {indicePunto + 1}?\nVerifica que la zona esté despejada.",
                "Confirmar Desplazamiento", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                this.Text = $"HMI JAKA - Desplazando a Punto {indicePunto + 1}...";
                JKTYPE.CartesianPose destino = JKTYPE.CartesianPose.withRadians(p.PoseFinal[0], p.PoseFinal[1], p.PoseFinal[2], p.PoseFinal[3], p.PoseFinal[4], p.PoseFinal[5]);

                Task.Run(() =>
                {
                    int res = jakaAPI.linear_move(ref robotHandle, ref destino, 0, true, p.VelNorm);
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (res == 0) this.Text = $"HMI JAKA - Detenido en Punto {indicePunto + 1}";
                        else MessageBox.Show("Error durante el movimiento lineal.", "Error.");
                    });
                });
            }
        }
        private void IrPto1_Click(object sender, EventArgs e) { MoverAPuntoGuardado(0); }
        private void IrPto2_Click(object sender, EventArgs e) { MoverAPuntoGuardado(1); }
        private void IrPto3_Click(object sender, EventArgs e) { MoverAPuntoGuardado(2); }
        private void IrPto4_Click(object sender, EventArgs e) { MoverAPuntoGuardado(3); }
        private void IrPto5_Click(object sender, EventArgs e) { MoverAPuntoGuardado(4); }
        private void IrPto6_Click(object sender, EventArgs e) { MoverAPuntoGuardado(5); }
        private void IrPto7_Click(object sender, EventArgs e) { MoverAPuntoGuardado(6); }
        private void IrPto8_Click(object sender, EventArgs e) { MoverAPuntoGuardado(7); }
        private void IrPto9_Click(object sender, EventArgs e) { MoverAPuntoGuardado(8); }
        private void IrPto10_Click(object sender, EventArgs e) { MoverAPuntoGuardado(9); }

        private void BtnAceptarTrayec_Click(object sender, EventArgs e)
        {
            if (!RobotListo()) return;

            JKTYPE.JointValue posicionActualStruct = new JKTYPE.JointValue();
            int res = jakaAPI.get_joint_position(ref robotHandle, ref posicionActualStruct);
            if (res == 0)
            {
                if (cmbListaTray.Text == "Agregar Trayectoria")
                {
                    string nombre = txtNombreTrayecto.Text.Trim();
                    if (string.IsNullOrEmpty(nombre))
                    { MessageBox.Show("Por favor, escribe un nombre para el trayecto.", "Requisitos faltantes."); return; }

                    if (cmbCantidadPuntos.SelectedIndex == -1)
                    { MessageBox.Show("Selecciona de cuántos puntos consta el trayecto.", "Requisitos faltantes."); return; }

                    modoEdicion = false;
                    int puntosA_Grabar = int.Parse(cmbCantidadPuntos.SelectedItem.ToString());
                    ConfigurarVisibilidadFilas(puntosA_Grabar, true);

                    for (int i = 0; i < 10; i++)
                    { puntosInterfazFijos[i] = new Punto { Velocidad = 30.0, OffsetMm = 0.0, Herramienta = cmbTools.Text }; }

                    this.Text = $"HMI JAKA - Grabando: {nombre} ({puntosA_Grabar} Puntos)";
                    MessageBox.Show($"Mueva el robot a cada posición y use los botones 'Capturar' correspondientes.", "Grabando.");

                }
                else if (cmbListaTray.SelectedIndex == -1)
                { MessageBox.Show("Error al iniciar trayectoria. Elija algo en la lista de trayectorias. ", "Error."); }

                else
                {
                    int indiceTrayectoriaAEditar = cmbListaTray.SelectedIndex - 1;

                    if (indiceTrayectoriaAEditar < 0 || indiceTrayectoriaAEditar >= historialTrayectorias.Count)
                    { MessageBox.Show("La trayectoria seleccionada no coincide con los registros de la memoria." + historialTrayectorias, "Error."); return; }

                    modoEdicion = false;
                    this.indiceTrayectoriaAEditar = indiceTrayectoriaAEditar;
                    Trayectoria t = historialTrayectorias[indiceTrayectoriaAEditar];

                    txtNombreTrayecto.Enabled = false;
                    txtNombreTrayecto.Text = t.Nombre;
                    cmbCantidadPuntos.SelectedItem = t.Pasos.Count.ToString();

                    ConfigurarVisibilidadFilas(t.Pasos.Count, false);

                    ComboBox[] combosVel = new ComboBox[] { cmbVTP1, cmbVTP2, cmbVTP3, cmbVTP4, cmbVTP5, cmbVTP6, cmbVTP7, cmbVTP8, cmbVTP9, cmbVTP10 };
                    ComboBox[] combosOff = new ComboBox[] { cmbMOP1, cmbMOP2, cmbMOP3, cmbMOP4, cmbMOP5, cmbMOP6, cmbMOP7, cmbMOP8, cmbMOP9, cmbMOP10 };
                    ComboBox[] combosVN = new ComboBox[] { cmbVN1, cmbVN2, cmbVN3, cmbVN4, cmbVN5, cmbVN6, cmbVN7, cmbVN8, cmbVN9, cmbVN10 };

                    for (int i = 0; i < t.Pasos.Count; i++)
                    {
                        puntosInterfazFijos[i] = t.Pasos[i];
                        combosVN[i].Text = t.Pasos[i].VelNorm.ToString();
                        combosVel[i].Text = t.Pasos[i].Velocidad.ToString();
                        combosOff[i].Text = t.Pasos[i].OffsetMm.ToString();
                    }
                    this.Text = $"HMI JAKA- Viendo: {t.Nombre}";
                }
            }
        }
        private void BtnGuardarTrayecto_Click(object sender, EventArgs e)
        {
            Herramienta seleccionada = (Herramienta)cmbTools.SelectedItem;
            bool PuntosCapturados = true;
            string nombre = txtNombreTrayecto.Text.Trim();
            if (string.IsNullOrEmpty(nombre) || cmbCantidadPuntos.SelectedIndex == -1)
            { MessageBox.Show("Faltan campos obligatorios.", "Falta Informacion."); return; }

            if (seleccionada != null)
            {
                JKTYPE.CartesianPose Coor = seleccionada.DatosCalibracion;
                if (jakaAPI.set_tool_data(ref robotHandle, seleccionada.Id, ref Coor, seleccionada.Nombre) != 0)
                { MessageBox.Show("Seleccionar herramienta", "Requisitos faltantes."); }
            }

            int totalPuntos = int.Parse(cmbCantidadPuntos.SelectedItem.ToString());
            Trayectoria tResult = new Trayectoria() { Nombre = nombre };

            ComboBox[] combosVel = new ComboBox[] { cmbVTP1, cmbVTP2, cmbVTP3, cmbVTP4, cmbVTP5, cmbVTP6, cmbVTP7, cmbVTP8, cmbVTP9, cmbVTP10 };
            ComboBox[] combosOff = new ComboBox[] { cmbMOP1, cmbMOP2, cmbMOP3, cmbMOP4, cmbMOP5, cmbMOP6, cmbMOP7, cmbMOP8, cmbMOP9, cmbMOP10 };
            ComboBox[] combosVN = new ComboBox[] { cmbVN1, cmbVN2, cmbVN3, cmbVN4, cmbVN5, cmbVN6, cmbVN7, cmbVN8, cmbVN9, cmbVN10 };
            Button[] lzPto = new Button[] { lzP1, lzP2, lzP3, lzP4, lzP5, lzP6, lzP7, lzP8, lzP9, lzP10 };
            if (modoEdicion == false)
            {
                for (int i = 0; i < totalPuntos; i++)
                { PuntosCapturados = true; if (lzPto[i].BackColor != Color.Green) { PuntosCapturados = false; break; } }
            }
            if (PuntosCapturados == true)
            {
                DialogResult Mens = MessageBox.Show("Quieres guardar los datos del trayecto?.", "Asegurar guardar.", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (Mens == DialogResult.Yes)
                {
                    for (int i = 0; i < totalPuntos; i++)
                    {
                        Punto puntoActual = puntosInterfazFijos[i];

                        if (combosVel[i].SelectedItem != null)
                            puntoActual.Velocidad = Convert.ToDouble(combosVel[i].SelectedItem.ToString());
                        if (combosOff[i].SelectedItem != null)
                            puntoActual.OffsetMm = Convert.ToDouble(combosOff[i].SelectedItem.ToString());
                        if (combosVN[i].SelectedItem != null)
                            puntoActual.VelNorm = Convert.ToDouble(combosVN[i].SelectedItem.ToString());
                        tResult.Pasos.Add(puntosInterfazFijos[i]);
                    }
                    txtNombreTrayecto.Clear();
                    cmbCantidadPuntos.SelectedIndex = -1;
                    ConfigurarVisibilidadFilas(0);
                    if (modoEdicion)
                    {
                        historialTrayectorias[indiceTrayectoriaAEditar] = tResult;
                        modoEdicion = false;
                        txtNombreTrayecto.Enabled = true;
                        cmbListaTray.SelectedItem = null;
                        indiceTrayectoriaAEditar = -1;
                        btnEditarTrayecto.Text = "Editar";
                        this.Text = "HMI JAKA - La trayectoria '{nombre}' fue editada y guardada.";
                    }
                    else
                    {
                        historialTrayectorias.Add(tResult);
                        cmbListaTray.Items.Add(nombre);
                        cmbLista.Items.Add(nombre);
                        cmbListaTray.SelectedItem = null;
                        this.Text = "HMI JAKA - Nueva trayectoria '{nombre}' guardada.";
                    }
                    guardado = 1;
                }
            }
            else { MessageBox.Show("Asegurate de capturar todos los puntos.", "Error al guardar"); }
        }
        private void BtnEditarTrayecto_Click(object sender, EventArgs e)
        {
            if (btnEditarTrayecto.Text == "Editar")
            {
                if (cmbListaTray.SelectedIndex == -1)
                { MessageBox.Show("Selecciona una trayectoria de la lista.", "Requisitos faltantes."); return; }

                if (cmbListaTray.Text == "Agregar Trayectoria")
                { MessageBox.Show("Selecciona una trayectoria guardada de la lista.", "Requisitos no aceptados."); return; }

                if (cmbCantidadPuntos.SelectedIndex == -1)
                { MessageBox.Show("Selecciona una trayectoria y presiona aceptar.", "Requisitos faltantes."); return; }

                int indiceAEditar = cmbListaTray.SelectedIndex - 1;
                if (indiceAEditar >= 0 && indiceAEditar < historialTrayectorias.Count)
                {
                    modoEdicion = true;
                    guardado = -1;
                    this.indiceTrayectoriaAEditar = indiceAEditar;
                    Trayectoria t = historialTrayectorias[this.indiceTrayectoriaAEditar];

                    ConfigurarVisibilidadFilas(t.Pasos.Count, true);
                    this.Text = $"HMI JAKA - EDITANDO: {t.Nombre}";
                    btnEditarTrayecto.Text = "Modo edicion";
                }
            }
            else
            {
                int indiceAEditar = cmbListaTray.SelectedIndex - 1;
                if (indiceAEditar >= 0 && indiceAEditar < historialTrayectorias.Count)
                {
                    modoEdicion = false;
                    guardado = 1;
                    this.indiceTrayectoriaAEditar = indiceAEditar;
                    Trayectoria t = historialTrayectorias[this.indiceTrayectoriaAEditar];

                    ConfigurarVisibilidadFilas(t.Pasos.Count, false);
                    this.Text = $"HMI JAKA - QUITANDO MODO EDICION: {t.Nombre}";
                    btnEditarTrayecto.Text = "Editar";
                    MessageBox.Show($"Modo edicion desactivado para {t.Nombre}");
                }
            }
        }
        private void BtnDetener_Click(object sender, EventArgs e)
        {
            DialogResult detener = MessageBox.Show("Seguro que quieres detener la trayectoria? ", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (detener == DialogResult.Yes)
            {
                if (lblStatusConexion.Text == "Desconectado") return;
                try
                {
                    int resultadoAbortar = jakaAPI.motion_abort(ref robotHandle);
                    if (resultadoAbortar == 0)
                    {
                        jakaAPI.disable_robot(ref robotHandle);
                        this.Text = "HMI JAKA - Movimiento Abortado";
                        MessageBox.Show("Se ha enviado parada de emergencia.", "Error.");
                        MessageBox.Show("El robot se a apagado y deshannilitado.", "Aviso.");
                        btnInterruptor.Enabled = false;
                        btnHabilitar.Enabled = false;
                    }
                }
                catch (Exception ex) { MessageBox.Show($"Error al detener: {ex.Message}", "Error al abortar."); }
            }
            else { MessageBox.Show("No se a detenido el robot.", "Detener cancelado."); }
        }
        private void BtnAjustes_Click(object sender, EventArgs e) { pnlAjustes.Visible = true; }
        private void BtnSalidaAjustes_Click_1(object sender, EventArgs e) { pnlAjustes.Visible = false; }
        private void BtnSalidaTrayectorias_Click(object sender, EventArgs e)
        {
            if (cmbListaTray.Text != "")
            {
                if (guardado == -1)
                {
                    DialogResult GuardarSalir = MessageBox.Show("Seguro que quieres salir sin guardar?", "Anuncio", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (GuardarSalir == DialogResult.Yes)
                    { CerrarTrayectorias(); }
                }
                else { CerrarTrayectorias(); }
            }
            else { CerrarTrayectorias(); }
        }
        private void BtnIrTrayectos_Click(object sender, EventArgs e) { pnlTrayectorias.Visible = true; pnlAjustes.Visible = false; }
        private void BtnIO_Click(object sender, EventArgs e) { pnlIO.Visible = true; pnlAjustes.Visible = false; }
        private void BtnSalidaIO_Click(object sender, EventArgs e) { pnlIO.Visible = false; TimerRep.Start(); pnlAjustes.Visible = true; }
        private void btnSalidaControl_Click(object sender, EventArgs e) { pnlControl.Visible = false; pnlTrayectorias.Visible = true; }
        private void CmbListaTray_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbListaTray.Text == "Agregar Trayectoria")
            {
                ConfigurarVisibilidadFilas(0, true);
                txtNombreTrayecto.Clear();
                cmbCantidadPuntos.SelectedIndex = -1;
            }
        }
        private void PnlIO_VisibleChanged(object sender, EventArgs e)
        {
            TimerIO.Enabled = pnlIO.Visible;
            if (!pnlIO.Visible && luzEntradas != null)
            {
                TimerRep.Stop();
                for (int i = 0; i < 10; i++)
                { if (luzEntradas[i] != null) luzEntradas[i].BackColor = SystemColors.ScrollBar; }
            }
        }

        private void RobotDesconectado()
        {
            lblStatusConexion.Text = "Desconectado"; lblStatusConexion.ForeColor = Color.DarkRed;
            btnConectar.Text = "Conectar al Robot"; btnConectar.BackColor = Color.CornflowerBlue;
            lblStatus.Text = "Apagado"; lzStatus.BackColor = Color.DarkRed;
            btnInterruptor.Text = "Encender"; btnInterruptor.Enabled = false;
            lblHabi.Text = "Deshabilitado"; lzHabi.BackColor = Color.DarkRed;
            btnHabilitar.Text = "Habilitar"; btnHabilitar.Enabled = false;
            lblEstadoHome.Text = "Desconocido"; lzHome.BackColor = Color.DarkRed;
        }
        private void ConfigurarVisibilidadFilas(int puntosVisibles, bool permitirEdicion = true)
        {
            Control[] filasBotones = new Control[] { btnCapPto1, btnCapPto2, btnCapPto3, btnCapPto4, btnCapPto5, btnCapPto6, btnCapPto7, btnCapPto8, btnCapPto9, btnCapPto10 };
            Control[] combosVel = new Control[] { cmbVTP1, cmbVTP2, cmbVTP3, cmbVTP4, cmbVTP5, cmbVTP6, cmbVTP7, cmbVTP8, cmbVTP9, cmbVTP10 };
            Control[] combosOff = new Control[] { cmbMOP1, cmbMOP2, cmbMOP3, cmbMOP4, cmbMOP5, cmbMOP6, cmbMOP7, cmbMOP8, cmbMOP9, cmbMOP10 };
            Control[] combosIr = new Control[] { IrPto1, IrPto2, IrPto3, IrPto4, IrPto5, IrPto6, IrPto7, IrPto8, IrPto9, IrPto10 };
            Control[] combosVN = new Control[] { cmbVN1, cmbVN2, cmbVN3, cmbVN4, cmbVN5, cmbVN6, cmbVN7, cmbVN8, cmbVN9, cmbVN10 };
            Control[] labelPto = new Control[] { Pto1, Pto2, Pto3, Pto4, Pto5, Pto6, Pto7, Pto8, Pto9, Pto10 };
            Control[] lzPto = new Control[] { lzP1, lzP2, lzP3, lzP4, lzP5, lzP6, lzP7, lzP8, lzP9, lzP10 };
            for (int i = 0; i < 10; i++)
            {
                bool debeMostrarse = i < puntosVisibles;
                filasBotones[i].Visible = debeMostrarse;
                combosIr[i].Visible = debeMostrarse;
                combosVel[i].Visible = debeMostrarse;
                combosOff[i].Visible = debeMostrarse;
                combosVN[i].Visible = debeMostrarse;
                labelPto[i].Visible = debeMostrarse;
                lzPto[i].Visible = debeMostrarse;

                if (debeMostrarse)
                {
                    combosVel[i].Enabled = permitirEdicion;
                    combosOff[i].Enabled = permitirEdicion;
                    combosVN[i].Enabled = permitirEdicion;
                    filasBotones[i].Enabled = permitirEdicion;
                }
                else
                {
                    if (combosVel[i] is ComboBox cv) cv.SelectedIndex = -1;
                    if (combosOff[i] is ComboBox co) co.SelectedIndex = -1;
                    if (combosVN[i] is ComboBox cn) cn.SelectedIndex = -1;
                    if (lzPto[i] is Button cz) cz.BackColor = SystemColors.Control;
                }
            }
            cmbCantidadPuntos.Enabled = permitirEdicion;
            cmbTools.Enabled = permitirEdicion;
            btnFinalizarTrayecto.Enabled = permitirEdicion;
        }
        private void BtnCHome_Click(object sender, EventArgs e)
        {
            if (!RobotListo()) return;
            if (MessageBox.Show("¿Deseas cambiar la posición de HOME actual?", "Modificar Home", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                JKTYPE.JointValue posicionActualStruct = new JKTYPE.JointValue();

                int resJoint = jakaAPI.get_joint_position(ref robotHandle, ref posicionActualStruct);

                if (resJoint == 0)
                {
                    posicionHome = posicionActualStruct.jVal;
                    MessageBox.Show("Nuevo HOME guardado.", "Exito.");
                }
                else { MessageBox.Show("Error al leer la posicion del robot para cambiar HOME.", "Error"); }
            }
        }
        private void BtnHome_Click(object sender, EventArgs e)
        {
            if (!RobotListo()) return;
            JKTYPE.JointValue posicionActualStruct = new JKTYPE.JointValue();
            int res = jakaAPI.get_joint_position(ref robotHandle, ref posicionActualStruct);
            if (res == 0)
            {
                double[] articulacionesActuales = posicionActualStruct.jVal;
                if (VerificarSiEstaEnHome(articulacionesActuales, posicionHome))
                { this.Invoke((MethodInvoker)delegate { this.Text = "HMI JAKA - El robot ya esta en HOME"; }); }
                else
                {
                    if (MessageBox.Show("¿Mandar robot a HOME?", "Mover", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        this.Text = "HMI JAKA - Moviendo a HOME...";
                        JKTYPE.JointValue jValHome = new JKTYPE.JointValue() { jVal = posicionHome };
                        if (jakaAPI.joint_move(ref robotHandle, ref jValHome, JKTYPE.MoveMode.ABS, false, 0.3) == 0) this.Text = "HMI JAKA - En HOME";
                    }
                }
            }
        }
        private bool VerificarSiEstaEnHome(double[] posicionActual, double[] posicionHome)
        {
            if (posicionActual.Length != posicionHome.Length) return false;
            double tolerancia = 0.05;
            for (int i = 0; i < posicionActual.Length; i++)
            { if (Math.Abs(posicionActual[i] - posicionHome[i]) > tolerancia) return false; }
            return true;
        }
        private void CerrarTrayectorias()
        {
            pnlTrayectorias.Visible = false;
            pnlAjustes.Visible = true;
            ConfigurarVisibilidadFilas(0, true);
            cmbListaTray.SelectedIndex = -1;
            cmbCantidadPuntos.SelectedIndex = -1;
            txtNombreTrayecto.Clear();
            guardado = -1;
            this.Text = "HMI JAKA";
        }

        private void Indicadores(Control indicador, EstadoPaso estado)
        {
            if (indicador.InvokeRequired) { indicador.Invoke(new Action(() => Indicadores(indicador, estado))); return; }
            switch (estado)
            {
                case EstadoPaso.Inactivo: indicador.BackColor = Color.Silver; break;

                case EstadoPaso.EnProceso: indicador.BackColor = Color.Yellow; break;

                case EstadoPaso.Completado: indicador.BackColor = Color.Green; break;

                case EstadoPaso.Error: indicador.BackColor = Color.Red; break;
            }
        }
        private void pnlControlPunto( int p) { pnlTrayectorias.Visible = false; pnlControl.Visible = true; lblTrackBar.Text = velocidadPorcentaje.ToString("0") + "%"; punto = p; MessageBox.Show("Dar click para mover y dar otro click para parar el robot.", "Aviso."); lblPunto.Text = "Capturando Punto " + (punto + 1);  }
        private void ResetearLuces() { Indicadores(lzInició, EstadoPaso.Inactivo); Indicadores(lzLlegoAPunto, EstadoPaso.Inactivo); Indicadores(lzAtornillado, EstadoPaso.Inactivo); Indicadores(lzConTornillo, EstadoPaso.Inactivo); }
        private void trackBarVelocidad_Scroll(object sender, EventArgs e) { velocidadPorcentaje = trackBarVelocidad.Value; }

        private void btnControlesClick(int Eje, double direccion)
        {
            if (Eje == -1) return;
            if (!enMovimiento) {
                double velocidad = 100.0;
                double velocidadActual = velocidad * (velocidadPorcentaje / 100);
                double distancia = 200;
                double distanciaFinal = distancia * direccion;
                enMovimiento = true;
                jakaAPI.jog(ref robotHandle, Eje, JKTYPE.MoveMode.INCR, JKTYPE.CoordType.COORD_TOOL, velocidadActual, distanciaFinal);
            } else { jakaAPI.jog_stop(ref robotHandle, Eje); enMovimiento = false; }
        }


        private void btnMenX_Click(object sender, EventArgs e) { btnControlesClick(0, 1); }
        private void btnMasX_Click(object sender, EventArgs e) { btnControlesClick(0, -1); }
        private void btnMenY_Click(object sender, EventArgs e) { btnControlesClick(1, 1); }
        private void btnMasY_Click(object sender, EventArgs e) { btnControlesClick(1, -1); }
        private void btnMenZ_Click(object sender, EventArgs e) { btnControlesClick(2, 1); }
        private void btnMasZ_Click(object sender, EventArgs e) { btnControlesClick(2, -1); }
        private void btnMenRx_Click(object sender, EventArgs e) { btnControlesClick(3, 1); }
        private void btnMasRx_Click(object sender, EventArgs e) { btnControlesClick(3, -1); }
        private void btnMenRy_Click(object sender, EventArgs e) { btnControlesClick(4, 1); }
        private void btnMasRy_Click(object sender, EventArgs e) { btnControlesClick(4, -1); }
        private void btnMenRz_Click(object sender, EventArgs e) { btnControlesClick(5, 1); }
        private void btnMasRz_Click(object sender, EventArgs e) { btnControlesClick(5, -1); }    
        
        public class Herramienta
        {
            public string Nombre { get; set; }
            public int Id { get; set; }
            public JKTYPE.CartesianPose DatosCalibracion { get; set; }
            public Herramienta(string nombre, int id, double x, double y, double z, double rx, double ry, double rz)
            {
                Nombre = nombre;
                Id = id;
                DatosCalibracion = JKTYPE.CartesianPose.withRadians(x, y, z, rx, ry, rz);
            }
        }
        public class Punto
        {
            public double[] PoseFinal { get; set; }
            public double Velocidad { get; set; }
            public double VelNorm { get; set; }
            public double OffsetMm { get; set; }
            public string Herramienta { get; set; }
            public Punto() { PoseFinal = new double[6]; }
        }
        public class Trayectoria
        {
            public string Nombre { get; set; }
            public List<Punto> Pasos { get; set; }
            public Trayectoria() { Pasos = new List<Punto>(); }
            public override string ToString()
            { return string.IsNullOrEmpty(this.Nombre) ? "Sin Nombre" : this.Nombre; }
        }

    }
}
