<template>
  <div> 
    <div class="navbar navbar-dark bg-dark">
      <div class="container d-flex justify-content-between">
        <a href="#" class="navbar-brand">Centare AWS Image Manager Demo</a>
      </div>
    </div>

    <div id="app">
      <image-list></image-list>
    </div>
  </div>
</template>

<script>
  require('bootstrap/dist/css/bootstrap.css');
  require('./../static/imagemanager.css');

  import ImageList from './components/ImageList'
  import ImageThumbnail from './components/ImageThumbnail'
  import ImageUploader from './components/ImageUploader'

  export default {
    name: 'app',
    components: {
      ImageList,
      ImageThumbnail,
      ImageUploader
    }
  }
</script>