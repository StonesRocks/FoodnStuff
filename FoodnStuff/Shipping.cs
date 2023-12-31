﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.DataFormats;

namespace FoodnStuff
{
    public partial class Shipping : Form
    {
        private ProductManager productManager = ProductManager.GetInstance();
        public Shipping()
        {
            InitializeComponent();
            UpdateListBoxes();
        }
        private void shipOrderButton_Click(object sender, EventArgs e)
        {
            Transport chosenTransport = transportsListBox.SelectedItem as Transport;

            if (chosenTransport != null)
            {
                productManager.SendTransport(chosenTransport);
                chosenTransport.OrdersOutOnDelivery.Clear();
            }
            UpdateListBoxes();
        }
        private void removeFromReadyToBeShippedButton_Click(object sender, EventArgs e)
        {
            Transport selectedTransport = transportsListBox.SelectedItem as Transport;

            if (selectedTransport != null && selectedTransport.OrdersOutOnDelivery.Count > 0)
            {
                Order selectedOrder = readyToBeShippedListBox.SelectedItem as Order;

                if (selectedOrder != null)
                {
                    productManager.Orders.Add(selectedOrder);
                    selectedTransport.OrdersOutOnDelivery.Remove(selectedOrder);
                    selectedTransport.Capacity -= selectedOrder.CheckTotalOrderQuantity();

                    UpdateListBoxes();
                }
            }
            var fileManager = FileManager.GetInstance();
            fileManager.SaveManagers();
        }
        private void deleteOrderButton_Click(object sender, EventArgs e)
        {
            Order orderToDelete = ordersListBox.SelectedItem as Order;

            if (orderToDelete != null)
            {
                DialogResult deleteOrder = MessageBox.Show("Are you sure you want to delete this order?"
                 , "Delete Order", MessageBoxButtons.YesNo);

                if (deleteOrder == DialogResult.Yes)
                {

                    productManager.Orders.Remove(orderToDelete);
                    UpdateListBoxes();
                }
            }
        }
        private void ordersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Order orderToLoad = ordersListBox.SelectedItem as Order;

            if (orderToLoad != null)
            {
                productManager.CheckTransportAvailability(orderToLoad);
                transportsListBox.Visible = true;
                label2.Visible = true;

                transportsListBox.DisplayMember = "Info";
                transportsListBox.DataSource = new BindingSource(productManager.Transports, null);
            }
        }
        private void transportsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Transport chosenTransport = transportsListBox.SelectedItem as Transport;

            if (chosenTransport != null)
            {
                readyToBeShippedListBox.DisplayMember = "Info";
                readyToBeShippedListBox.DataSource = new BindingSource(chosenTransport.OrdersOutOnDelivery, null);

                readyToBeShippedListBox.Visible = true;
                shipOrderButton.Visible = true;
                removeFromReadyToBeShippedButton.Visible = true;
                label3.Visible = true;
            }
        }
        private void addButton_Click(object sender, EventArgs e)
        {
            Order orderToLoad = ordersListBox.SelectedItem as Order;
            Transport chosenTransport = transportsListBox.SelectedItem as Transport;

            if (orderToLoad != null && chosenTransport != null)
            {
                bool loadSuccess = productManager.LoadTansport(chosenTransport, orderToLoad);

                if (loadSuccess)
                {
                    productManager.Orders.Remove(orderToLoad);
                    UpdateListBoxes();
                }
            }
            var fileManager = FileManager.GetInstance();
            fileManager.SaveManagers();
        }

        private void UpdateListBoxes()
        {
            ordersListBox.DisplayMember = "Info";
            ordersListBox.DataSource = new BindingSource(productManager.Orders, null);

            transportsListBox.DisplayMember = "Info";
            transportsListBox.DataSource = new BindingSource(productManager.Transports, null);

            Transport chosenTransport = transportsListBox.SelectedItem as Transport;

            if (chosenTransport != null)
            {
                readyToBeShippedListBox.DisplayMember = "Info";
                readyToBeShippedListBox.DataSource = new BindingSource(chosenTransport.OrdersOutOnDelivery, null);
            }
        }
    }
}
