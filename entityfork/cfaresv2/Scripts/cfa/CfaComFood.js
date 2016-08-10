﻿

/*//////////////////////////////////////////////////////////////////////////////////////
////// Autogenerated by JaySvcUtil.exe http://JayData.org for more info        /////////
//////                             oData V3                                    /////////
//////////////////////////////////////////////////////////////////////////////////////*/
(function(global, $data, undefined) {

  $data.Entity.extend('StoriesEntityModel.Food', {
    'food_id': { 'key':true,'type':'Edm.Int32','nullable':false,'computed':true },
    'short_name': { 'type':'Edm.String','maxLength':180 },
    'food_name': { 'type':'Edm.String','maxLength':180 },
    'extended_name': { 'type':'Edm.String','maxLength':180 },
    'url_name': { 'type':'Edm.String','nullable':false,'required':true,'maxLength':180 },
    'url_hint': { 'type':'Edm.String','maxLength':180 },
    'order_hint': { 'type':'Edm.Int32','nullable':false,'required':true },
    'img': { 'type':'Edm.String','maxLength':Number.POSITIVE_INFINITY },
    'dom_id': { 'type':'Edm.String','nullable':false,'required':true,'maxLength':200 },
    'description': { 'type':'Edm.String','maxLength':Number.POSITIVE_INFINITY },
    'ingredients': { 'type':'Edm.String','maxLength':Number.POSITIVE_INFINITY },
    'calories': { 'type':'Edm.Int32' },
    'calories_fat': { 'type':'Edm.Int32' },
    'protein': { 'type':'Edm.Double' },
    'carbs': { 'type':'Edm.Double' },
    'carbs_daily': { 'type':'Edm.Double' },
    'sugar': { 'type':'Edm.Double' },
    'sugar_daily': { 'type':'Edm.Double' },
    'cholesterol': { 'type':'Edm.Double' },
    'cholesterol_daily': { 'type':'Edm.Double' },
    'sodium': { 'type':'Edm.Double' },
    'sodium_daily': { 'type':'Edm.Double' },
    'calcium': { 'type':'Edm.Double' },
    'calcium_daily': { 'type':'Edm.Double' },
    'iron': { 'type':'Edm.Double' },
    'iron_daily': { 'type':'Edm.Double' },
    'fiber': { 'type':'Edm.Double' },
    'fiber_daily': { 'type':'Edm.Double' },
    'fat': { 'type':'Edm.Double' },
    'fat_daily': { 'type':'Edm.Double' },
    'trans_fat': { 'type':'Edm.Double' },
    'trans_fat_daily': { 'type':'Edm.Double' },
    'saturated_fat': { 'type':'Edm.Double' },
    'saturated_fat_daily': { 'type':'Edm.Double' },
    'vitamin_a': { 'type':'Edm.Double' },
    'vitamin_a_daily': { 'type':'Edm.Double' },
    'vitamin_c': { 'type':'Edm.Double' },
    'vitamin_c_daily': { 'type':'Edm.Double' },
    'serving_size': { 'type':'Edm.Double' },
    'serving_size_international': { 'type':'Edm.Double' },
    'parent_item': { 'type':'Edm.Int32' },
    'non_menu': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'no_image': { 'type':'Edm.Boolean','nullable':false,'required':true },
    'notice': { 'type':'Edm.String','maxLength':Number.POSITIVE_INFINITY },
    'show_serving': { 'type':'Edm.Boolean','nullable':false,'required':true }
  });
  
  $data.Entity.extend('StoriesEntityModel.FoodImage', {
    'image_id': { 'key':true,'type':'Edm.Int32','nullable':false,'computed':true },
    'name': { 'type':'Edm.String','nullable':false,'required':true,'maxLength':250 },
    'img_url_name': { 'type':'Edm.String','nullable':false,'required':true,'maxLength':250 },
    'src': { 'type':'Edm.String','nullable':false,'required':true,'maxLength':250 },
    'width': { 'type':'Edm.Int32','nullable':false,'required':true },
    'height': { 'type':'Edm.Int32','nullable':false,'required':true },
    'brief': { 'type':'Edm.String','maxLength':Number.POSITIVE_INFINITY },
    'ratio': { 'type':'Edm.Double','nullable':false,'required':true },
    'size': { 'type':'Edm.Int32','nullable':false,'required':true },
    'food_id': { 'type':'Edm.Int32','nullable':false,'required':true }
  });
  
  $data.EntityContext.extend('cfacom.entity.dao.Designer.CfaComStoriesEntities', {
    'Foods': { type: $data.EntitySet, elementType: StoriesEntityModel.Food },
    'FoodImages': { type: $data.EntitySet, elementType: StoriesEntityModel.FoodImage }
  });

  $data.generatedContexts = $data.generatedContexts || [];
  $data.generatedContexts.push(cfacom.entity.dao.Designer.CfaComStoriesEntities);
  
  /*Context Instance*/
  cfacom.entity.dao.Designer.context = new cfacom.entity.dao.Designer.CfaComStoriesEntities( { name:'oData', oDataServiceHost: 'http://core.chick-fil-a.com/DataServices/Food.svc' });

      
})(window, $data);
      
    