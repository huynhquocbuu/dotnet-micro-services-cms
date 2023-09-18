
```shell
ng new my-blog --routing --style=scss
ng new angular-routing-scully --routing --style=scss
```

angular bootstrap
```shell
npm install bootstrap
```
Add the following import statement at the top of the styles.scss file that exists in the
src folder of our Angular application
```css
@import "bootstrap/scss/bootstrap";
```

create module and component in module
```shell
ng generate module core
ng generate component header --path=src/app/core --module=core --export
```