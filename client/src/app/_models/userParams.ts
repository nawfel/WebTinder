import { User } from "./user";

export class UserParams {
    gender: string;
    minAge :number;
    maxAge : number;
    pageNumber = 1;
    pageSize = 20;
    orderBy ='lastActive'
    constructor(user: User) {
      //  this.gender = user.gender == 'female' ? 'male' : 'female';
        this.gender =  'female';
        this.minAge=18;
        this.maxAge=100
    }
}