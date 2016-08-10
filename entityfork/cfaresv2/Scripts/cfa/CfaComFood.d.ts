﻿///<reference path="./jaydata.d.ts" />
/*//////////////////////////////////////////////////////////////////////////////////////
////// Autogenerated by JaySvcUtil.exe http://JayData.org for more info        /////////
//////                      oData V3 TypeScript                                /////////
//////////////////////////////////////////////////////////////////////////////////////*/


module StoriesEntityModel {
  class Food extends $data.Entity {
    constructor ();
    constructor (initData: { food_id?: number; short_name?: string; food_name?: string; extended_name?: string; url_name?: string; url_hint?: string; order_hint?: number; img?: string; dom_id?: string; description?: string; ingredients?: string; calories?: number; calories_fat?: number; protein?: number; carbs?: number; carbs_daily?: number; sugar?: number; sugar_daily?: number; cholesterol?: number; cholesterol_daily?: number; sodium?: number; sodium_daily?: number; calcium?: number; calcium_daily?: number; iron?: number; iron_daily?: number; fiber?: number; fiber_daily?: number; fat?: number; fat_daily?: number; trans_fat?: number; trans_fat_daily?: number; saturated_fat?: number; saturated_fat_daily?: number; vitamin_a?: number; vitamin_a_daily?: number; vitamin_c?: number; vitamin_c_daily?: number; serving_size?: number; serving_size_international?: number; parent_item?: number; non_menu?: bool; no_image?: bool; notice?: string; show_serving?: bool; });
    food_id: number;
    short_name: string;
    food_name: string;
    extended_name: string;
    url_name: string;
    url_hint: string;
    order_hint: number;
    img: string;
    dom_id: string;
    description: string;
    ingredients: string;
    calories: number;
    calories_fat: number;
    protein: number;
    carbs: number;
    carbs_daily: number;
    sugar: number;
    sugar_daily: number;
    cholesterol: number;
    cholesterol_daily: number;
    sodium: number;
    sodium_daily: number;
    calcium: number;
    calcium_daily: number;
    iron: number;
    iron_daily: number;
    fiber: number;
    fiber_daily: number;
    fat: number;
    fat_daily: number;
    trans_fat: number;
    trans_fat_daily: number;
    saturated_fat: number;
    saturated_fat_daily: number;
    vitamin_a: number;
    vitamin_a_daily: number;
    vitamin_c: number;
    vitamin_c_daily: number;
    serving_size: number;
    serving_size_international: number;
    parent_item: number;
    non_menu: bool;
    no_image: bool;
    notice: string;
    show_serving: bool;
    
  }

  export interface FoodQueryable extends $data.Queryable {
    filter(predicate:(it: StoriesEntityModel.Food) => bool): StoriesEntityModel.FoodQueryable;
    filter(predicate:(it: StoriesEntityModel.Food) => bool, thisArg: any): StoriesEntityModel.FoodQueryable;

    map(projection: (it: StoriesEntityModel.Food) => any): StoriesEntityModel.FoodQueryable;

    length(): $data.IPromise;
    length(handler: (result: number) => void): $data.IPromise;
    length(handler: { success?: (result: number) => void; error?: (result: any) => void; }): $data.IPromise;

    forEach(handler: (it: StoriesEntityModel.Food) => void ): $data.IPromise;
    
    toArray(): $data.IPromise;
    toArray(handler: (result: StoriesEntityModel.Food[]) => void): $data.IPromise;
    toArray(handler: { success?: (result: StoriesEntityModel.Food[]) => void; error?: (result: any) => void; }): $data.IPromise;

    single(predicate: (it: StoriesEntityModel.Food, params?: any) => bool, params?: any, handler?: (result: StoriesEntityModel.Food) => void): $data.IPromise;
    single(predicate: (it: StoriesEntityModel.Food, params?: any) => bool, params?: any, handler?: { success?: (result: StoriesEntityModel.Food[]) => void; error?: (result: any) => void; }): $data.IPromise;

    take(amout: number): StoriesEntityModel.FoodQueryable;
    skip(amout: number): StoriesEntityModel.FoodQueryable;

    order(selector: string): StoriesEntityModel.FoodQueryable;
    orderBy(predicate: (it: StoriesEntityModel.Food) => any): StoriesEntityModel.FoodQueryable;
    orderByDescending(predicate: (it: StoriesEntityModel.Food) => any): StoriesEntityModel.FoodQueryable;
    
    first(predicate: (it: StoriesEntityModel.Food, params?: any) => bool, params?: any, handler?: (result: StoriesEntityModel.Food) => void): $data.IPromise;
    first(predicate: (it: StoriesEntityModel.Food, params?: any) => bool, params?: any, handler?: { success?: (result: StoriesEntityModel.Food[]) => void; error?: (result: any) => void; }): $data.IPromise;
    
    include(selector: string): StoriesEntityModel.FoodQueryable;
    withInlineCount(): StoriesEntityModel.FoodQueryable;
    withInlineCount(selector: string): StoriesEntityModel.FoodQueryable;

    removeAll(): $data.IPromise;
    removeAll(handler: (count: number) => void): $data.IPromise;
    removeAll(handler: { success?: (result: number) => void; error?: (result: any) => void; }): $data.IPromise;
  }


  export interface FoodSet extends FoodQueryable {
    add(initData: { food_id?: number; short_name?: string; food_name?: string; extended_name?: string; url_name?: string; url_hint?: string; order_hint?: number; img?: string; dom_id?: string; description?: string; ingredients?: string; calories?: number; calories_fat?: number; protein?: number; carbs?: number; carbs_daily?: number; sugar?: number; sugar_daily?: number; cholesterol?: number; cholesterol_daily?: number; sodium?: number; sodium_daily?: number; calcium?: number; calcium_daily?: number; iron?: number; iron_daily?: number; fiber?: number; fiber_daily?: number; fat?: number; fat_daily?: number; trans_fat?: number; trans_fat_daily?: number; saturated_fat?: number; saturated_fat_daily?: number; vitamin_a?: number; vitamin_a_daily?: number; vitamin_c?: number; vitamin_c_daily?: number; serving_size?: number; serving_size_international?: number; parent_item?: number; non_menu?: bool; no_image?: bool; notice?: string; show_serving?: bool; }): StoriesEntityModel.Food;
    add(item: StoriesEntityModel.Food): StoriesEntityModel.Food;

    attach(item: StoriesEntityModel.Food): void;
    attach(item: { food_id: number; }): void;
    attachOrGet(item: StoriesEntityModel.Food): StoriesEntityModel.Food;
    attachOrGet(item: { food_id: number; }): StoriesEntityModel.Food;

    detach(item: StoriesEntityModel.Food): void;
    detach(item: { food_id: number; }): void;

    remove(item: StoriesEntityModel.Food): void;
    remove(item: { food_id: number; }): void;
    
    elementType: new (initData: { food_id?: number; short_name?: string; food_name?: string; extended_name?: string; url_name?: string; url_hint?: string; order_hint?: number; img?: string; dom_id?: string; description?: string; ingredients?: string; calories?: number; calories_fat?: number; protein?: number; carbs?: number; carbs_daily?: number; sugar?: number; sugar_daily?: number; cholesterol?: number; cholesterol_daily?: number; sodium?: number; sodium_daily?: number; calcium?: number; calcium_daily?: number; iron?: number; iron_daily?: number; fiber?: number; fiber_daily?: number; fat?: number; fat_daily?: number; trans_fat?: number; trans_fat_daily?: number; saturated_fat?: number; saturated_fat_daily?: number; vitamin_a?: number; vitamin_a_daily?: number; vitamin_c?: number; vitamin_c_daily?: number; serving_size?: number; serving_size_international?: number; parent_item?: number; non_menu?: bool; no_image?: bool; notice?: string; show_serving?: bool; }) => StoriesEntityModel.Food;
  }

  class FoodImage extends $data.Entity {
    constructor ();
    constructor (initData: { image_id?: number; name?: string; img_url_name?: string; src?: string; width?: number; height?: number; brief?: string; ratio?: number; size?: number; food_id?: number; });
    image_id: number;
    name: string;
    img_url_name: string;
    src: string;
    width: number;
    height: number;
    brief: string;
    ratio: number;
    size: number;
    food_id: number;
    
  }

  export interface FoodImageQueryable extends $data.Queryable {
    filter(predicate:(it: StoriesEntityModel.FoodImage) => bool): StoriesEntityModel.FoodImageQueryable;
    filter(predicate:(it: StoriesEntityModel.FoodImage) => bool, thisArg: any): StoriesEntityModel.FoodImageQueryable;

    map(projection: (it: StoriesEntityModel.FoodImage) => any): StoriesEntityModel.FoodImageQueryable;

    length(): $data.IPromise;
    length(handler: (result: number) => void): $data.IPromise;
    length(handler: { success?: (result: number) => void; error?: (result: any) => void; }): $data.IPromise;

    forEach(handler: (it: StoriesEntityModel.FoodImage) => void ): $data.IPromise;
    
    toArray(): $data.IPromise;
    toArray(handler: (result: StoriesEntityModel.FoodImage[]) => void): $data.IPromise;
    toArray(handler: { success?: (result: StoriesEntityModel.FoodImage[]) => void; error?: (result: any) => void; }): $data.IPromise;

    single(predicate: (it: StoriesEntityModel.FoodImage, params?: any) => bool, params?: any, handler?: (result: StoriesEntityModel.FoodImage) => void): $data.IPromise;
    single(predicate: (it: StoriesEntityModel.FoodImage, params?: any) => bool, params?: any, handler?: { success?: (result: StoriesEntityModel.FoodImage[]) => void; error?: (result: any) => void; }): $data.IPromise;

    take(amout: number): StoriesEntityModel.FoodImageQueryable;
    skip(amout: number): StoriesEntityModel.FoodImageQueryable;

    order(selector: string): StoriesEntityModel.FoodImageQueryable;
    orderBy(predicate: (it: StoriesEntityModel.FoodImage) => any): StoriesEntityModel.FoodImageQueryable;
    orderByDescending(predicate: (it: StoriesEntityModel.FoodImage) => any): StoriesEntityModel.FoodImageQueryable;
    
    first(predicate: (it: StoriesEntityModel.FoodImage, params?: any) => bool, params?: any, handler?: (result: StoriesEntityModel.FoodImage) => void): $data.IPromise;
    first(predicate: (it: StoriesEntityModel.FoodImage, params?: any) => bool, params?: any, handler?: { success?: (result: StoriesEntityModel.FoodImage[]) => void; error?: (result: any) => void; }): $data.IPromise;
    
    include(selector: string): StoriesEntityModel.FoodImageQueryable;
    withInlineCount(): StoriesEntityModel.FoodImageQueryable;
    withInlineCount(selector: string): StoriesEntityModel.FoodImageQueryable;

    removeAll(): $data.IPromise;
    removeAll(handler: (count: number) => void): $data.IPromise;
    removeAll(handler: { success?: (result: number) => void; error?: (result: any) => void; }): $data.IPromise;
  }


  export interface FoodImageSet extends FoodImageQueryable {
    add(initData: { image_id?: number; name?: string; img_url_name?: string; src?: string; width?: number; height?: number; brief?: string; ratio?: number; size?: number; food_id?: number; }): StoriesEntityModel.FoodImage;
    add(item: StoriesEntityModel.FoodImage): StoriesEntityModel.FoodImage;

    attach(item: StoriesEntityModel.FoodImage): void;
    attach(item: { image_id: number; }): void;
    attachOrGet(item: StoriesEntityModel.FoodImage): StoriesEntityModel.FoodImage;
    attachOrGet(item: { image_id: number; }): StoriesEntityModel.FoodImage;

    detach(item: StoriesEntityModel.FoodImage): void;
    detach(item: { image_id: number; }): void;

    remove(item: StoriesEntityModel.FoodImage): void;
    remove(item: { image_id: number; }): void;
    
    elementType: new (initData: { image_id?: number; name?: string; img_url_name?: string; src?: string; width?: number; height?: number; brief?: string; ratio?: number; size?: number; food_id?: number; }) => StoriesEntityModel.FoodImage;
  }

}

module cfacom.entity.dao.Designer {
  export class CfaComStoriesEntities extends $data.EntityContext {
    onReady(): $data.IPromise;
    onReady(handler: (context: CfaComStoriesEntities) => void): $data.IPromise;
    Foods: StoriesEntityModel.FoodSet;
    FoodImages: StoriesEntityModel.FoodImageSet;
    
  }
}
