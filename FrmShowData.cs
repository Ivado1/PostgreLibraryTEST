using Npgsql;
using System;
using System.Data;

using System.Windows.Forms;
using System.Diagnostics;

namespace Library_App
{
    public partial class FrmShowData : Form
    {
        //Простой вариант для теста
        string vStrConnection = "Server=localhost ; port=5432 ; user id=postgres ; password=postgres ; database=testdb ;";

        NpgsqlConnection postgreConnect = new NpgsqlConnection();
        NpgsqlCommand postgreCommand = new NpgsqlCommand();
        NpgsqlDataReader postgreReader;

        public FrmShowData()
        {
            InitializeComponent();
        }
        
        //Открыть подключение
        private void OpenConnect()
        {
            if (postgreConnect != null && postgreConnect.State == ConnectionState.Closed)
            {
                postgreConnect.ConnectionString = vStrConnection;
                postgreConnect.Open();
            }
        }
        //Закрыть подключение
        private void CloseConnect()
        {
            if (postgreConnect != null && postgreConnect.State != ConnectionState.Closed)
            {
                postgreConnect.Close();
            }
        }

        //Отобразить базовые таблицы при запуске
        private void FrmShowData_Load(object sender, EventArgs e)
        {
            postgreConnect.ConnectionString = vStrConnection;
            OpenConnect();

            postgreCommand = new NpgsqlCommand("SELECT book_id,title,author_name,author_lastname,published, borrowed, publisher_id  FROM book", postgreConnect);
            postgreReader = postgreCommand.ExecuteReader();
            
            if (postgreReader.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(postgreReader);
                dataGridView_slct_book.DataSource = dt;
            }

            postgreCommand = new NpgsqlCommand("SELECT user_id,login,pass,admin,debtor FROM users", postgreConnect);
            postgreReader = postgreCommand.ExecuteReader();

            if (postgreReader.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(postgreReader);
                dataGridView_slct_user.DataSource = dt;
            }

            if (postgreCommand != null) postgreCommand.Dispose();
            CloseConnect();
        }

        //Добавить издателя в БД библеотеки
        private void insertBtn_Click(object sender, EventArgs e)
        {
            if (postgreConnect.State == ConnectionState.Closed) postgreConnect.Open();

            if (!string.IsNullOrEmpty(address_box_insert.Text) &&!string.IsNullOrWhiteSpace(address_box_insert.Text)
                &&!string.IsNullOrEmpty(id_box_insert.Text) && !string.IsNullOrWhiteSpace(id_box_insert.Text)
                && !string.IsNullOrEmpty(publisher_box_insert.Text) && !string.IsNullOrWhiteSpace(publisher_box_insert.Text))
            {
                postgreCommand = new NpgsqlCommand("INSERT INTO publisher (publisher_id, org_name, address) VALUES(@publisher_id, @org_name, @address)", postgreConnect);

                postgreCommand.Parameters.AddWithValue("publisher_id", Convert.ToInt32(id_box_insert.Text));
                postgreCommand.Parameters.AddWithValue("org_name", publisher_box_insert.Text);
                postgreCommand.Parameters.AddWithValue("address", address_box_insert.Text);

                postgreCommand.ExecuteNonQuery();
            }
            else
            {
                error_text_insert.Visible = true;
                error_text_insert.Text = "Заполни значения полей";
            }

            postgreCommand = new NpgsqlCommand("SELECT publisher_id, org_name,address FROM publisher", postgreConnect);
            postgreReader = postgreCommand.ExecuteReader();

            if (postgreReader.HasRows)
            {
                Debug.WriteLine("Update data");
                DataTable data = new DataTable();
                data.Load(postgreReader);
                dataGridView1.DataSource = data;
            }
            if (postgreCommand != null) postgreCommand.Dispose();
            CloseConnect();
        }

        //Изменить информацию об издателя 
        private void updateBtn_Click(object sender, EventArgs e)
        {
            if (postgreConnect.State == ConnectionState.Closed) postgreConnect.Open();

            if (!string.IsNullOrEmpty(text_address_update.Text) &&!string.IsNullOrWhiteSpace(text_address_update.Text)
                && !string.IsNullOrEmpty(text_id_update.Text) && !string.IsNullOrWhiteSpace(text_id_update.Text)
                && !string.IsNullOrEmpty(text_publisher_update.Text) && !string.IsNullOrWhiteSpace(text_address_update.Text))
            {
                postgreCommand = new NpgsqlCommand("UPDATE publisher SET org_name=@org_name, address=@address WHERE publisher_id=@publishr_id", postgreConnect);
                postgreCommand.Parameters.AddWithValue("org_name", text_publisher_update.Text);
                postgreCommand.Parameters.AddWithValue("address", text_address_update.Text);
                postgreCommand.Parameters.AddWithValue("publishr_id",Convert.ToInt32(text_id_update.Text));

                postgreCommand.ExecuteNonQuery();
            }
            else if(string.IsNullOrEmpty(text_id_update.Text) && string.IsNullOrWhiteSpace(text_id_update.Text))
            {
                error_text_update.Visible = true;
                error_text_update.Text = "Укажи ID книги";
                //error_text_update.Text = "Нет названия книги";
            }
            else 
            {
                error_text_update.Visible = true;
                error_text_update.Text = "Напиши название книги и издателя";
            }
            CloseConnect();
        }

        //Удалить издателя 
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            OpenConnect();
            if (!string.IsNullOrEmpty(text_id_delete.Text) && !string.IsNullOrWhiteSpace(text_id_delete.Text))
            {
                postgreCommand = new NpgsqlCommand("DELETE FROM publisher WHERE publisher_id=@publisher_id", postgreConnect); // call @ value
                postgreCommand.Parameters.AddWithValue("publisher_id", Convert.ToInt32(text_id_delete.Text)); // call @ value

                postgreCommand.ExecuteNonQuery();
            }
            postgreCommand.Dispose();
            CloseConnect();
        }
        
        //Событие закрыть форму
        private void FrmShowData_FormClose(object sender, FormClosingEventArgs e)
        {
            CloseConnect();
        }

        //Изменить отображаемую таблицу через комбобокс
        private void comboBox_Tables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (postgreConnect.State == ConnectionState.Closed) postgreConnect.Open();
            
            switch (comboBox_Tables.SelectedItem)
            {
                case "book":
                    postgreCommand = new NpgsqlCommand("SELECT book_id,title,author_name,author_lastname,published, borrowed, publisher_id  FROM book", postgreConnect);
                    break;
                case "publisher":
                    postgreCommand = new NpgsqlCommand("SELECT publisher_id, org_name,address FROM publisher", postgreConnect);
                    break;
                case "users":
                    postgreCommand = new NpgsqlCommand("SELECT user_id,login,pass,admin,debtor FROM users", postgreConnect);
                    break;
                default:error_text_insert.Text = "Нет такой базы данных";
                    break;
            }
            
            postgreReader = postgreCommand.ExecuteReader();

            if (postgreReader.HasRows)
            {
                DataTable data = new DataTable();
                data.Load(postgreReader);
                dataGridView1.DataSource = data;
            }
            
            if (postgreReader != null) postgreReader.Close();

            CloseConnect();
        }

        //Добавить книгу в БД библеотеки
        private void btn_Select_Click(object sender, EventArgs e)
        {
            OpenConnect();

            if (!string.IsNullOrEmpty(text_ID_book_SLCT.Text) && !string.IsNullOrWhiteSpace(text_ID_book_SLCT.Text))
            {
                postgreCommand = new NpgsqlCommand("SELECT book_id,title,author_name,author_lastname,published, borrowed, publisher_id FROM book WHERE book_id=@book_id", postgreConnect);
                postgreCommand.Parameters.AddWithValue("book_id", Convert.ToInt32(text_ID_book_SLCT.Text));
                postgreReader=postgreCommand.ExecuteReader();
                
                if (postgreReader.HasRows)
                {
                    DataTable data = new DataTable();
                    data.Load(postgreReader);
                    dataGridView_slct_book.DataSource = data;
                }
                else
                {
                    error_SELECT.Visible = true;
                    error_SELECT.Text = "Книги с таким ID нет";
                }
            }
            else if (!string.IsNullOrEmpty(text_Title_SELECT.Text) && !string.IsNullOrWhiteSpace(text_Title_SELECT.Text))
            {
                postgreCommand = new NpgsqlCommand("SELECT book_id,title,author_name,author_lastname,published, borrowed, publisher_id FROM book WHERE title=@title", postgreConnect);

                postgreCommand.Parameters.AddWithValue("title", text_Title_SELECT.Text);
                postgreReader = postgreCommand.ExecuteReader();

                if (postgreReader.HasRows)
                {
                    DataTable data = new DataTable();
                    data.Load(postgreReader);
                    dataGridView_slct_book.DataSource = data;
                }
                else
                {
                    error_SELECT.Visible = true;
                    error_SELECT.Text = "Книги с таким названием нет";
                }
            }
            else if (!string.IsNullOrEmpty(text_LastName_SELECT.Text) && !string.IsNullOrWhiteSpace(text_LastName_SELECT.Text))
            {
                postgreCommand = new NpgsqlCommand("SELECT * FROM book WHERE author_lastname=@lastname", postgreConnect);
                postgreCommand.Parameters.AddWithValue("lastname", text_LastName_SELECT.Text);
                postgreReader = postgreCommand.ExecuteReader();

                if (postgreReader.HasRows)
                {
                    DataTable data = new DataTable();
                    data.Load(postgreReader);
                    dataGridView_slct_book.DataSource = data;
                }
                else
                {
                    error_SELECT.Visible = true;
                    error_SELECT.Text = "Такого автора нет";
                }
            }
            else
            {
                postgreCommand = new NpgsqlCommand("SELECT book_id,title,author_name,author_lastname,published, borrowed, publisher_id FROM book", postgreConnect);
                postgreReader = postgreCommand.ExecuteReader();

                if (postgreReader.HasRows)
                {
                    DataTable dt = new DataTable();
                    dt.Load(postgreReader);
                    dataGridView_slct_book.DataSource = dt;
                }

                error_SELECT.Visible = true;
                error_SELECT.Text = "Заполни значения полей";
            }

            if (postgreCommand != null) postgreCommand.Dispose();
            CloseConnect();
        }

        //Добавить книгу в БД библеотеки
        private void btn_insrt_book_Click(object sender, EventArgs e)
        {
            OpenConnect();

            if (!string.IsNullOrEmpty(text_insrt_title.Text)&&!string.IsNullOrEmpty(text_insrt_Name.Text)&&
                !string.IsNullOrEmpty(text_insrt_family.Text)&&!string.IsNullOrEmpty(text_insrt_datePubl.Text))
            {
                postgreCommand = new NpgsqlCommand("INSERT INTO book (book_id, title, author_name ,author_lastname, published) VALUES(@id, @title, @name, @family, @datePub)", postgreConnect);

                postgreCommand.Parameters.AddWithValue("id", 22);
                postgreCommand.Parameters.AddWithValue("title", text_insrt_title.Text);
                postgreCommand.Parameters.AddWithValue("name", text_insrt_Name.Text);
                postgreCommand.Parameters.AddWithValue("family", text_insrt_family.Text);
                postgreCommand.Parameters.AddWithValue("datePub", Convert.ToDateTime(text_insrt_datePubl.Text)); // paste good
                postgreCommand.ExecuteNonQuery();

                error_insrtbook.Visible = true;
                error_insrtbook.Text = ("книга добавлена");
            }
            else
            {
                error_insrtbook.Visible = true;
                error_insrtbook.Text=("заполни все поля");
            }

            postgreCommand = new NpgsqlCommand("SELECT book_id,title,author_name,author_lastname,published, borrowed, publisher_id FROM book", postgreConnect);
            postgreReader = postgreCommand.ExecuteReader();

            if (postgreReader.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(postgreReader);
                dataGridView_insrtbook.DataSource = dt;
            }
            
            if (postgreCommand != null) postgreCommand.Dispose();
            CloseConnect();
        }
        
        //Удалить нужную книгу 
        private void btn_dlt_book_Click(object sender, EventArgs e)
        {
            OpenConnect();
            try
            {
                postgreCommand = new NpgsqlCommand("DELETE FROM book WHERE book_id=@id", postgreConnect);
                postgreCommand.Parameters.AddWithValue("id", Convert.ToInt32(text_dlt_book.Text));
                postgreCommand.ExecuteNonQuery();

                error_dlt_book.Visible = true;
                error_dlt_book.Text = "книга с id № " + text_dlt_book.Text + " удалена";
            }
            catch 
            {
                error_dlt_book.Visible = true;
                error_dlt_book.Text = "книга с id № " + text_dlt_book.Text + " нет!";
            }

            postgreCommand.Dispose();
            CloseConnect();
        }

        //Отобразить таблицу читателей
        private void btn_slct_user_Click(object sender, EventArgs e)
        {
            OpenConnect();

            if (!string.IsNullOrEmpty(text_id_user_slct.Text))
            {
                postgreCommand = new NpgsqlCommand("SELECT book_id,title,author_name,author_lastname,published, borrowed, publisher_id FROM users WHERE user_id=@id", postgreConnect);
                postgreCommand.Parameters.AddWithValue("id", Convert.ToInt32(text_id_user_slct.Text));

                postgreReader = postgreCommand.ExecuteReader();
                if (postgreReader.HasRows)
                {
                    DataTable dt = new DataTable();
                    dt.Load(postgreReader);
                    dataGridView_slct_user.DataSource = dt;
                }
                else
                {
                    error_slct_user.Visible = true;
                    error_slct_user.Text = "Читателей с таким id нет";
                }
            }
            else
            {
                postgreCommand = new NpgsqlCommand("SELECT user_id,login,pass,admin,debtor FROM users", postgreConnect);
                postgreReader = postgreCommand.ExecuteReader();

                if (postgreReader.HasRows)
                {
                    DataTable dt = new DataTable();
                    dt.Load(postgreReader);
                    dataGridView_slct_user.DataSource = dt;
                }
            }

            if (postgreCommand != null) postgreCommand.Dispose();
            CloseConnect();
        }

        //Отобразить таблицу выданных книг
        private void btn_brrw_Click(object sender, EventArgs e)
        {
            OpenConnect();

            postgreCommand = new NpgsqlCommand("SELECT title, author, borrow_date, return_date, user_id,login,borrow_id,book_id FROM borrow", postgreConnect);
            postgreReader = postgreCommand.ExecuteReader();

            if (postgreReader.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(postgreReader);
                dataGridView_borrow.DataSource = dt;
            }
            if (postgreCommand != null) postgreCommand.Dispose();
            CloseConnect();
        }
        
        //Кнопка для адмнистратора выдать книгу если указана книга и читатель 
        private void btn_give_book_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(text_ID_book_SLCT.Text) && !string.IsNullOrEmpty(text_id_user_slct.Text))
            {
                set_bookid();
                set_userid();
                create_borrow();
            }
            else
            {
                notice_msg.Visible = true;
                notice_msg.Text = "заполни значение полей";
            }
        }
        
        //Отметит книгу как выдано читателю 
        private void set_bookid()
        {
            OpenConnect();

            postgreCommand = new NpgsqlCommand("UPDATE book SET borrowed=true WHERE book_id=@book_id", postgreConnect);
            postgreCommand.Parameters.AddWithValue("book_id", Convert.ToInt32(text_ID_book_SLCT.Text));
            postgreCommand.ExecuteReader();

            if (postgreCommand != null) postgreCommand.Dispose();
            CloseConnect();
        }
        
        //Отметит читателя что он взял книгу 
        private void set_userid()
        {
            OpenConnect();

            postgreCommand = new NpgsqlCommand("UPDATE users SET debtor=true WHERE user_id=@user_id", postgreConnect); 
            postgreCommand.Parameters.AddWithValue("user_id", Convert.ToInt32(text_id_user_slct.Text));
            postgreCommand.ExecuteReader();

            if (postgreCommand != null) postgreCommand.Dispose();
            CloseConnect();
        }
        
        // Создаст заказ с пометкой кому была выдана конкретная книга в нужную дату  
        private void create_borrow()
        {
            OpenConnect();
            
            postgreCommand = new NpgsqlCommand("INSERT INTO borrow (title, author, borrow_date, return_date, user_id, login, book_id) SELECT title,author_lastname,@borrow_date,@return_date,@user_id,@login,@book_id FROM book WHERE @book_id=book_id", postgreConnect);
            postgreCommand.Parameters.AddWithValue("borrow_date", DateTime.Now);
            postgreCommand.Parameters.AddWithValue("return_date", DateTime.Now.AddDays(14));
            postgreCommand.Parameters.AddWithValue("user_id", Convert.ToInt32(text_id_user_slct.Text));
            postgreCommand.Parameters.AddWithValue("login", "logggg");
            postgreCommand.Parameters.AddWithValue("book_id", Convert.ToInt32(text_ID_book_SLCT.Text));
            postgreCommand.ExecuteReader();

            notice_msg.Visible = true;
            notice_msg.Text = "КНИГА № " + text_ID_book_SLCT.Text + " ВЫДАНА, ЧИТАТЕЛЮ ID № " + text_id_user_slct.Text;
            MessageBox.Show("КНИГА № " + text_ID_book_SLCT.Text + " ВЫДАНА, ЧИТАТЕЛЮ ID № " + text_id_user_slct.Text);
            if (postgreCommand != null) postgreCommand.Dispose();
            CloseConnect();
        }
    }
}
