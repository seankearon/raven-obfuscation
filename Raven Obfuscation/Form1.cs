using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Raven_Obfuscation
{
    public partial class Form1 : Form
    {
        private readonly EmbeddableDocumentStore store;

        public Form1()
        {
            InitializeComponent();

            store = new EmbeddableDocumentStore { UseEmbeddedHttpServer = true };
            store.Initialize();

            var peopleByName = new PeopleByName { Conventions = new DocumentConvention() };
            var definition = peopleByName.CreateIndexDefinition();

            MessageBox.Show(definition.Map);

            peopleByName.Execute(store);
        }

        private void QueryOverTheIndex()
        {
            using (var session = store.OpenSession())
            {
                Text = "Number of Seans = " + session.Query<Person, PeopleByName>().Customize(x => x.WaitForNonStaleResults()).Where(x => x.Name == "Sean").Take(100).Count();
            }
        }

        private void AddPerson()
        {
            using (var session = store.OpenSession())
            {
                session.Store(new Person{Name = "Sean" });
                session.SaveChanges();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddPerson();
            QueryOverTheIndex();
        }
    }

    public class PeopleByName: AbstractIndexCreationTask<Person>
    {
        public PeopleByName()
        {
            Map = persons =>
                  from p in persons
                  select new
                             {
                                 p.Name
                             };
        }
    }

    [Obfuscation(Exclude = true)]
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
