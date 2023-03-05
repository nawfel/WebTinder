import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs'
import { User } from '../_models/user';
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  
  baseurl = 'https://localhost:5001/api/'
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();
 
  constructor(private htpp: HttpClient) {}

  login(model: any) {
    return this.htpp.post<User>(this.baseurl + 'account/login', model).pipe(
      map((respone: User) => {
        const user = respone;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);

        }
      }
      )
    );
  }
  register(model:any){
    return this.htpp.post<User>(this.baseurl+'account/register',model).pipe(
      map(user=>{
        if(user){
          localStorage.setItem('user',JSON.stringify(user));
          this.currentUserSource.next(user);
        }
        return user;
      })
    )
  }
  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
