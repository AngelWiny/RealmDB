using RealmDB.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RealmDB
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var realmDB = Realm.GetInstance();
            List<Student> studentList = realmDB.All<Student>().ToList();
            lstStudents.ItemsSource = studentList;
        }

        List<Option> optionItems = new List<Option>();
        Student editStudent;

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            var realmDB = Realm.GetInstance();
            var students = realmDB.All<Student>().ToList();
            var maxStudentId = 0;
            if (students.Count != 0)
            {

                maxStudentId = students.Max(s => s.Id);
            }
            Student student = new Student()
            {
                Id = maxStudentId + 1,
                Name = txtNombre.Text
            };
            realmDB.Write(() =>
            {
                realmDB.Add(student);
            });
            txtNombre.Text = string.Empty;
            List<Student> studentList = realmDB.All<Student>().ToList();
            lstStudents.ItemsSource = studentList;
        }

        private async void optionList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var realmDB = Realm.GetInstance();
            Option selectedItem = optionList.SelectedItem as Option;
            if (selectedItem != null)
            {
                switch (selectedItem.OptionText)
                {
                    case "Edit":
                        popupOptionView.IsVisible = false;
                        popupEditView.IsVisible = true;
                        editStudent = realmDB.All<Student>().First(b => b.Id == selectedItem.StudentId);
                        txtEditName.Text = editStudent.Name;
                        break;

                    case "Delete":
                        var removeStudent = realmDB.All<Student>().First(b => b.Id == selectedItem.StudentId);
                        using (var db = realmDB.BeginWrite())
                        {
                            realmDB.Remove(removeStudent);
                            db.Commit();
                        }
                        await DisplayAlert("Success", "Student Deleted", "OK");
                        popupOptionView.IsVisible = false;
                        List<Student> studentList = realmDB.All<Student>().ToList();
                        lstStudents.ItemsSource = studentList;
                        break;

                    default:
                        popupOptionView.IsVisible = false;
                        break;
                }
                optionList.SelectedItem = null;
            }
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            popupEditView.IsVisible = false;
        }

        private async void btnEdit_Clicked(object sender, EventArgs e)
        {
            var realmDB = Realm.GetInstance();
            var selectedStudent = realmDB.All<Student>().First(b => b.Id == editStudent.Id);
            using (var db = realmDB.BeginWrite())
            {
                editStudent.Name = txtEditName.Text;
                db.Commit();
            }
            await DisplayAlert("Success", "Student Updated", "OK");
            txtEditName.Text = string.Empty;
            popupEditView.IsVisible = false;
        }

        private void lstStudents_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Student selectedStudent = lstStudents.SelectedItem as Student;
            if (selectedStudent != null)
            {
                optionItems.Clear();
                optionItems.Add(new Option { OptionText = "Edit", StudentId = selectedStudent.Id });
                optionItems.Add(new Option { OptionText = "Delete", StudentId = selectedStudent.Id });
                optionItems.Add(new Option { OptionText = "Cancel" });
                optionList.ItemsSource = optionItems;
                popupOptionView.IsVisible = true;
            }
        }
    }
}
