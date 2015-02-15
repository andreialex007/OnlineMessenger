definitions = [
   new FieldDefinition("Id", "Integer"),
   new FieldDefinition("Name", "String"),
   new FieldDefinition("UsersCount", "Count")
];

function RolesPage() {
    var self = new UsersPage();
    self.SearchUrl("/SearchRoles");
    self.DeleteUrl("/DeleteRoles");
    return self;
}