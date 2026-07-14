export interface LoginDto {
    email: string;
    password: string;
}

export interface RegisterDto {
    userName: string;
    password: string;
    firstName: string;
    lastName: string;
    email: string;
}

export interface AuthResponseDto {
    userId: number;
    userTypeId: number;
    userTypeName: string;
    userName: string;
    email: string;
    fullName: string;
}

export interface ChangePasswordDto {
    userId: number;
    currentPassword: string;
    newPassword: string;
}
