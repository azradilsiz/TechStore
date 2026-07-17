export interface Product {
    id: number;
    categoryId: number;
    categoryName: string;
    name: string;
    description: string;
    price: number;
    stock: number;
    imageUrl: string;
}

export interface ProductWriteDto {
    categoryId: number;
    name: string;
    description: string;
    price: number;
    stock: number;
    imageUrl: string;
}
