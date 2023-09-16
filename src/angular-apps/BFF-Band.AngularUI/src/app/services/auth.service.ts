import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {User} from "../models/user";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { }

  public register(user: User) : Observable<any>{

    return this.http.post<any>(
      'http://localhost:5145/api/Auth/register',
      user
    );
  }

  public login(user: User): Observable<string> {
    return this.http.post('http://localhost:5145/api/Auth/login', user, {
      responseType: 'text',
    });
  }

  public getMe(): Observable<string> {
    return this.http.get('http://localhost:5145/api/Auth', {
      responseType: 'text',
    });
  }
}
