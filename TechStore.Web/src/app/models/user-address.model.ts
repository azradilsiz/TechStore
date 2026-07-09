export interface UserAddress {
  id: number;
  userId: number;
  city: string;
  district: string;
  addressDetail: string;
  phone: string;
  title: string;
}

export interface CreateUserAddress {
  userId: number;
  city: string;
  district: string;
  addressDetail: string;
  phone: string;
  title: string;
}
