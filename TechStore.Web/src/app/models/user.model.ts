export interface User {
  id: number;
  userTypeId: number;
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
}

export type UserWriteDto = Omit<User, 'id'>;
