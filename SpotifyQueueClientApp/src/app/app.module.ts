import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { PagesModule } from './pages/pages.module';
import { ServicesModule } from './services/services.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from './helpers/jwt-interceptor';
import { ModalsModule } from './modals/modals.module';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PipesModule } from './pipes/pipes.module';
import { BsDropdownModule } from 'ngx-bootstrap';
import { PartyModule } from './features/party/party.module';
import { SharedModule } from './shared/shared.module';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    PagesModule,
    AppRoutingModule,
    ServicesModule,
    ModalModule.forRoot(),
    BsDropdownModule.forRoot(),
    ModalsModule,
    PipesModule,
    PartyModule,
    SharedModule
  ],
  exports: [
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
